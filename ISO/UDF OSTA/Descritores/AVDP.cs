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

//Anchor Volume Descriptor Pointer
public class AVDP: Descritor
{
    public Extensor VolumePrincipal, VolumeReserva;

    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(VolumePrincipal.GetData());
        outBin.AddRange(VolumeReserva.GetData());
        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);
        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 2,
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
            ID_de_Descritor = 2,
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

    public AVDP() { }
    public AVDP(byte[] Sector)
    {
        ReadDTAG(Sector);

        #region Leitura do Extensor
        VolumePrincipal.Tamanho_Dados = (int)Sector.ReadUInt(0x10, 32);
        VolumePrincipal.LBA_Dados = (int)Sector.ReadUInt(0x14, 32);

        VolumeReserva.Tamanho_Dados = (int)Sector.ReadUInt(0x18, 32);
        VolumeReserva.LBA_Dados = (int)Sector.ReadUInt(0x1C, 32);
        #endregion
    }

    public struct Extensor
    {
        public int Tamanho_Dados;//uint32
        public int LBA_Dados;//uint32

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(BitConverter.GetBytes((UInt32)Tamanho_Dados));
            outbin.AddRange(BitConverter.GetBytes((UInt32)LBA_Dados));
            return outbin.ToArray();
        }
    }
}
