using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021

//Partition Descriptor
public class PD: Descritor
{
    public uint DescritorVolumeSequencialNumber;
    public Flags Regra = Flags.NãoAlocado;

    public uint NúmeroPartições;
    public regid ConteúdodePartição;
    public byte[] UsoPartição;
    public uint TipoAcesso;

    public uint LBAPartição, TamanhoPartiçãoBlocks;
    public FSD FileSet;
    public TD FileSetTerminator;
    public regid IdImplementação;
    public byte[] UsoImplementação;
    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(BitConverter.GetBytes((UInt32)DescritorVolumeSequencialNumber));
        outBin.AddRange(BitConverter.GetBytes((UInt16)1));//REGRA
        outBin.AddRange(BitConverter.GetBytes((UInt16)NúmeroPartições));

        outBin.AddRange(ConteúdodePartição.GetData());
        outBin.AddRange(UsoPartição);

        outBin.AddRange(BitConverter.GetBytes((UInt32)TipoAcesso));
        outBin.AddRange(BitConverter.GetBytes((UInt32)LBAPartição));
        outBin.AddRange(BitConverter.GetBytes((UInt32)TamanhoPartiçãoBlocks));

        outBin.AddRange(IdImplementação.GetData());
        outBin.AddRange(UsoImplementação);
        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);
        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 5,
            LBA_This_Descritor = (uint)lba,
            Tamanho_CRC_Descritor = (uint)outBin.Count,
            Versão = 2,
            Reservado = 0,
            VolumeSerialNumber = 0,
            CRC_Descritor = UDFUtils.ComputeCrc(outBin.ToArray(), outBin.Count)
        }.GetTag());
        byte tagchecksum = UDFUtils.TagChecksum(outSector.ToArray());

        outSector.Clear();
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 5,
            LBA_This_Descritor = (uint)lba,
            Tamanho_CRC_Descritor = (uint)outBin.Count,
            Versão = 2,
            Reservado = 0,
            VolumeSerialNumber = 0,
            CRC_Descritor = UDFUtils.ComputeCrc(outBin.ToArray(), outBin.Count),
            TagChecksum = tagchecksum
        }.GetTag());

        outSector.AddRange(outBin);

        while (outSector.Count % tamanhosetor != 0 || outSector.Count() < tamanhosetor)
            outSector.Add(0);
        return outSector.ToArray();
    }

    public PD() { }
    public PD(byte[] Sector)
    {
        ReadDTAG(Sector);

        DescritorVolumeSequencialNumber = Sector.ReadUInt(0x10, 32);

        if (BitConverter.GetBytes(Sector.ReadUInt(0x14, 16))[0].ReadBit(0))
            Regra = Flags.Alocado;

        NúmeroPartições = Sector.ReadUInt(0x16, 16);
        ConteúdodePartição.ReadFromData(Sector.ReadBytes(0x18, 0x20));
        UsoPartição = Sector.ReadBytes(0x38, 0x80);

        TipoAcesso = Sector.ReadUInt(0xb8, 32);
        //0 - Não Especificado
        //1 - Apenas Leitura [RO]
        //2 - Escrever Uma Vez [W]
        //3 - Re-Escrita [RW]
        //4 - Sobrescrita [OW]
        //5... Reservado

        LBAPartição = Sector.ReadUInt(0xBC, 32);
        TamanhoPartiçãoBlocks = Sector.ReadUInt(0xC0, 32);//Tamanho da partição em blocos dos setores

        IdImplementação.ReadFromData(Sector.ReadBytes(0xC4, 0x20));
        UsoImplementação = Sector.ReadBytes(0xE4, 0x80);
    }

    public void CriarPasta(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    public void ExtractToFolder(string save, Stream ISO)
    {
        string path = save + @"\";

        var Entradas = FileSet.FileIdentifiers;

        foreach (var Entrada in Entradas)//Cada set de pastas
        {
            foreach (var Arquivo in Entrada.Where(x=>x.FileCaracteristics.Contains(
                FI.FileCaracteristic.Archive)).ToArray())//Cada arquivo
            {
                GetFilePath(Arquivo, Entradas);

                //MessageBox.Show(Arquivo.FileIdentifier.Dados);
            }
        }
    }
    public void EscreverArquivo(string path, FE arquivo, Stream ISO)
    {
        var writer = new FileStream(path, FileMode.Create);
        writer.WriteBytes(ISO, 0, (arquivo.DescritoresAlocação.LocalizaçãoExtent.LogicalBlockNumber+LBAPartição)*tamanhosetor,arquivo.DescritoresAlocação.TamanhoExtent);
        writer.Close();
    }
    public string GetFilePath(FI arquivo, List<FI[]> Entradas)
    {
        string path = "";

        List<FI> paths = new List<FI>();

        foreach(var Entrada in Entradas)
        {
            if(Entrada.Contains(arquivo))
            {
                paths.Add(GetEntry(Entradas,
                        Entrada[0].ICB.LocalizaçãoExtent.LogicalBlockNumber));

                int i = 0;
                while (!paths.Contains(Entradas[0][0]))//Enquanto não conter a raiz(destino final)
                {
                    paths.Add(GetEntry(Entradas, 
                        paths[i].ICB.LocalizaçãoExtent.LogicalBlockNumber));
                    i++;
                }
            }

        }
        for(int k=paths.Count-1;k>0; k--)
        {
            path += @"/" + paths[k].FileIdentifier.Dados;
        }
        return path;
    }

    public FI GetEntry(List<FI[]> Entradas, uint icblba)
    {
        FI folder = null;
        foreach(var entry in Entradas)
            foreach(var subentry in entry)
            {
                if (subentry.ICB.LocalizaçãoExtent.LogicalBlockNumber == icblba&&
                    subentry.FileCaracteristics.Contains(FI.FileCaracteristic.Directory))
                    folder = subentry;
            }
        return folder;
    }
    public enum Flags
    {
        NãoAlocado,
        Alocado
    };
}
