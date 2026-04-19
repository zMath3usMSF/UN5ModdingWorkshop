using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public class Setor
{
    public Stream iso;
    public int lba, tamanhosetor, offsetsetor;
    public Tipo_de_Descritor tipo = Tipo_de_Descritor.Outro;
    public enum Tipo_de_Descritor
    {
        BootRecord,
        VolumePrimário,
        Outro
    };

    public static byte[] GetStringEmptySector(byte id,string data, int tamanhosetor)
    {
        var outb = new List<byte>();
        outb.Add(id);
        outb.AddRange(Encoding.Default.GetBytes(data));
        while (outb.Count % tamanhosetor != 0 || outb.Count() < tamanhosetor)
            outb.Add(0);
        return outb.ToArray();
    }
    public string NomeSeção;
    public byte Versão;
    virtual public byte[] GetSectorData
    {
        get => iso.ReadSector(lba, tamanhosetor);
    }
    public static Setor ReadSector(Stream input, int lba, int tamanho)
    {
        Setor sektor = null;
        byte[] sector = input.ReadSector(lba, tamanho);
        if (sector.ReadBytes(1,5).ConvertTo(Encoding.Default)=="CD001"&&
            sector[0]==1)
        {
            sektor = new Volume_Primário(input, lba, tamanho);
        }
        else
        {
            sektor = new Desconhecido(lba, tamanho);
            if(sector[0]==0)
                sektor.NomeSeção = sector.ReadBytes(1, 5).ConvertTo(Encoding.Default);
        }
        return sektor;
    }
}

