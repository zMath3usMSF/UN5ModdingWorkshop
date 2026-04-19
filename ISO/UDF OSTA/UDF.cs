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

public class UDF
{
    public ISO9660 inheritISO;
    public List<Descritor> Setores;

    public Volume Principal, Reserva;

    public UDF(ISO9660 iso9660)
    {
        //Descritor de Ponteiro de Âncora de Volume --> Aponta todos os outros
        Setores = new List<Descritor>();
        Setores.Add(Descritor.ReadSector(iso9660.ISOfile, 256, iso9660.Tamanho_Setor));//Primeiro
        //Duplicata do Setor AVDP no último Setor, adicionar em criação[Leitura não necessária]
        AVDP aVDP = Setores[0] as AVDP;

        #region Volume Principal
        var setoresprinc = new List<Descritor>();
        for (int i = 0; i < aVDP.VolumePrincipal.Tamanho_Dados / iso9660.Tamanho_Setor; i++)
        {
            Descritor descritor = Descritor.ReadSector(iso9660.ISOfile, aVDP.VolumePrincipal.LBA_Dados+i, iso9660.Tamanho_Setor);
            setoresprinc.Add(descritor);
        }
        #region Separar na Struct
            foreach (Descritor descrit in setoresprinc)
            {
            if(descrit!=null)
                switch (descrit.GetType().ToString())
                {
                    case "PVD":
                        Principal.VolumePrimário = descrit as PVD;
                        break;
                    case "IUVD":
                        Principal.UsoImplementação = descrit as IUVD;
                        break;
                    case "PD":
                        Principal.Partição = descrit as PD;
                        Principal.Partição.FileSet = Descritor.ReadSector(iso9660.ISOfile, (int)Principal.Partição.LBAPartição, iso9660.Tamanho_Setor, Principal.Partição) as FSD;
                        Principal.Partição.FileSet.ReadFIS(iso9660.ISOfile, Principal.Partição);
                        break;
                    case "LV":
                        Principal.VolumeLógico = descrit as LV;
                        if (Principal.VolumeLógico.IntegritySequenceExtent.ExtentSize != 0 && Principal.VolumeLógico.IntegritySequenceExtent.LBAExtent != 0)
                            Principal.VolumeLógicoIntegridade = Descritor.ReadSector(iso9660.ISOfile, (int)Principal.VolumeLógico.IntegritySequenceExtent.LBAExtent, iso9660.Tamanho_Setor) as LVI;
                        break;
                    case "USD":
                        Principal.EspaçoNãoAlocado = descrit as USD;
                        break;
                    case "TD":
                        Principal.Terminador = descrit as TD;
                        break;
                }
            }
        #endregion
        #endregion
        Setores.AddRange(Principal.ToArray());

        #region Volume Reserva
        var setoresreserva = new List<Descritor>();
        for (int i = 0; i < aVDP.VolumeReserva.Tamanho_Dados / iso9660.Tamanho_Setor; i++)
        {
            Descritor descritor = Descritor.ReadSector(iso9660.ISOfile, aVDP.VolumeReserva.LBA_Dados + i, iso9660.Tamanho_Setor);
            setoresreserva.Add(descritor);
        }
        #region Separar na Struct
            foreach (Descritor descrit in setoresreserva)
            {
            if (descrit != null)
                switch (descrit.GetType().ToString())
                {
                    case "PVD":
                        Reserva.VolumePrimário = descrit as PVD;
                        break;
                    case "IUVD":
                        Reserva.UsoImplementação = descrit as IUVD;
                        break;
                    case "PD":
                        Reserva.Partição = descrit as PD;
                        Reserva.Partição.FileSet = Descritor.ReadSector(iso9660.ISOfile, (int)Principal.Partição.LBAPartição, iso9660.Tamanho_Setor, Reserva.Partição) as FSD;
                        Reserva.Partição.FileSet.ReadFIS(iso9660.ISOfile, Reserva.Partição);
                        break;
                    case "LV":
                        Reserva.VolumeLógico = descrit as LV;
                        if (Reserva.VolumeLógico.IntegritySequenceExtent.ExtentSize != 0 && Reserva.VolumeLógico.IntegritySequenceExtent.LBAExtent != 0)
                            Reserva.VolumeLógicoIntegridade = Descritor.ReadSector(iso9660.ISOfile, (int)Reserva.VolumeLógico.IntegritySequenceExtent.LBAExtent, iso9660.Tamanho_Setor) as LVI;
                        break;
                    case "USD":
                        Reserva.EspaçoNãoAlocado = descrit as USD;
                        break;
                    case "TD":
                        Reserva.Terminador = descrit as TD;
                        break;
                }
            }
        #endregion
        #endregion
        Setores.AddRange(Reserva.ToArray());

    }

    public void PatchVolume()
    {

    }

}
public struct Volume
{
    public PVD VolumePrimário;
    public IUVD UsoImplementação;
    public PD Partição;
    public LV VolumeLógico;
    public LVI VolumeLógicoIntegridade;
    public USD EspaçoNãoAlocado;
    public TD Terminador;

    public List<Descritor> ToArray()
    {
        var result = new List<Descritor>();
        result.Add(VolumePrimário);
        result.Add(UsoImplementação);
        result.Add(Partição);
        result.Add(VolumeLógico);
        result.Add(EspaçoNãoAlocado);
        result.Add(Terminador);
        return result;
    }
}
public static class UDFUtils
{
    //From DiscUtils UDF Library and OSTA/ECMA-167 Especifications
    private static readonly ushort[] CrcTable =
         {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
 0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
 0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
 0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
 0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
 0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
 0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
 0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
 0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
 0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
 0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
 0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
 0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
 0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
 0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
 0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
 0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
 0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
 0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
 0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
 0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
 0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
 0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
 0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
 0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
 0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
 0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
 0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
 0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
 0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
 0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
 0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0
        };
    public static ushort ComputeCrc(byte[] buffer, int length)
    {
        ushort crc = 0;

        int k = 0, len = length;
        while (len-- > 0)
            crc = (ushort)(CrcTable[(crc >> 8 ^ buffer[k++]) & 0xff] ^ (crc << 8));

        return crc;
    }
    public static ushort UnicodeChecksum(byte[] buffer, int length)
    {
        ushort crc = 0;

        int k = 0, len = length;
        while (len-- > 0)
        {
            //crc = (ushort)(CrcTable[(crc >> 8 ^ buffer[k++]) & 0xff] ^ (crc << 8));
            crc = (ushort)(CrcTable[(crc >> 8 ^ (buffer[k] >> 8) & 0xff)] ^ (crc << 8));
            crc = (ushort)(CrcTable[(crc >> 8 ^ (buffer[k++] & 0xff)) & 0xff] ^ (crc << 8));
        }
        return crc;
    }
    public static byte TagChecksum(byte[] buffer)
    {
        int result = 0;
        result = (int)(buffer[0] + buffer[1] + buffer[2] + buffer[3] + buffer[5] + buffer[6] + buffer[7] + buffer[8]
            + buffer[9] + buffer[10] + buffer[11] + buffer[12] + buffer[13] + buffer[14] + buffer[15]);
        return (byte)(result & 0xFF);
    }
    public static string ReadCompressedUnicode(byte[] buffer)
    {
        int numberOfBytes = buffer.Length; /* (Input) number of bytes read from media. */
        byte UDFCompressed = 0; /* (Input) bytes read from media. */
        uint[] unicode_t = new uint[numberOfBytes];/* (Output) uncompressed unicode characters. */

        uint compID;
        int returnValue, unicodeIndex, byteIndex;

        /* Use UDFCompressed to store current byte being read. */
        compID = buffer[0];

        /* First check for valid compID. */
        if (compID != 8 && compID != 16)
        {
            return "INVALID COMPRESSION!";
        }
        else
        {
            unicodeIndex = 0;
            byteIndex = 1;
            /* Loop through all the bytes. */
            while (byteIndex < numberOfBytes)
            {
                if (compID == 16)
                {
                    /*Move the first byte to the high bits of the unicode char. */
                    unicode_t[unicodeIndex] = (uint)(buffer[byteIndex++] << 8);
                }
                else
                {
                    unicode_t[unicodeIndex] = 0;
                }
                if (byteIndex < numberOfBytes)
                {
                    /*Then the next byte to the low bits. */
                    unicode_t[unicodeIndex] |= (uint)(buffer[byteIndex++]);
                }
                unicodeIndex++;
            }
        }

        var enc = new List<byte>();
        foreach (uint u in unicode_t)
            enc.AddRange(BitConverter.GetBytes((UInt16)u));
        return Encoding.Unicode.GetString(enc.ToArray());
    }
    public static string ReadDString(byte[] buffer, int offset, int count)
    {
        int byteLen = buffer[offset + count - 1];
        return ReadDCharacters(buffer, offset, byteLen);
    }
    public static string ReadDCharacters(byte[] buffer, int offset, int count)
    {
        if (count == 0)
        {
            return string.Empty;
        }

        byte alg = buffer[offset];

        if (alg != 8 && alg != 16)
        {
            throw new Exception("Corrupt compressed unicode string");
        }

        StringBuilder result = new StringBuilder(count);

        int pos = 1;
        while (pos < count)
        {
            char ch = '\0';

            if (alg == 16)
            {
                ch = (char)(buffer[offset + pos] << 8);
                pos++;
            }

            if (pos < count)
            {
                ch |= (char)buffer[offset + pos];
                pos++;
            }

            result.Append(ch);
        }

        return result.ToString();
    }
    private static short ForceRange(short min, short max, short val)
    {
        if (val < min)
        {
            return min;
        }
        if (val > max)
        {
            return max;
        }

        return val;
    }
    private static byte ForceRange(byte min, byte max, byte val)
    {
        if (val < min)
        {
            return min;
        }
        if (val > max)
        {
            return max;
        }

        return val;
    }
}
