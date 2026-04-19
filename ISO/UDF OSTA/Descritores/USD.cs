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

//Unallocated Space Descriptor
public class USD: Descritor
{
    public uint DescritorVolumeSequencialNumber;

    public uint AllocDescripNumber;
    public extent_ad[] DescritoresAlocação;
    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(BitConverter.GetBytes((UInt32)DescritorVolumeSequencialNumber));
        outBin.AddRange(BitConverter.GetBytes((UInt32)AllocDescripNumber));
        if (DescritoresAlocação.Length > 0)
            foreach (var desc in DescritoresAlocação)
                outBin.AddRange(desc.GetData());
        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);
        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 7,
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
            ID_de_Descritor = 7,
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

    public USD() { }
    public USD(byte[] Sector)
    {
        ReadDTAG(Sector);

        DescritorVolumeSequencialNumber = Sector.ReadUInt(0x10, 32);

        AllocDescripNumber = Sector.ReadUInt(0x14, 32);

        #region Extents Descritores Alocação
        var exts = new List<extent_ad>();
        for(uint i = 0x18; i < (AllocDescripNumber*8);i+=8)
        {
            byte[] extent = Sector.ReadBytes((int)i, 8);
            extent_ad ad = new extent_ad();
            ad.ReadfromData(extent);
            exts.Add(ad);
        }
        DescritoresAlocação = exts.ToArray();
        #endregion
    }
}
