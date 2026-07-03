using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Threading.Tasks;

/// <summary>
/// Classe de extensões de tipos de dados conhecidos, para usos diversos,
/// conversões e leituras.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
/// 
public static class IOextent
{
    /// <summary>
    /// Lê um float de 32 bits.[Stream].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Número de ponto flutuante(Single)</returns>
    public static Single ReadSingle(this byte[] array, int offset, bool bigendian = false)
    {
        byte[] data = array.Skip(offset).Take(4).ToArray(); // fix: Take(32) era errado, Single = 4 bytes
        if (bigendian) Array.Reverse(data);
        float result = BitConverter.ToSingle(data, 0);
        return result;
    }
    /// Lê um float de 32 bits e avança a posição em 4 bytes.[Stream].
    /// </summary>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Número de ponto flutuante(Single)</returns>
    public static Single ReadSingle(this Stream strean, bool bigendian = false)
    {
        byte[] bitssx = strean.ReadBytes(0, 4);
        if (bigendian == true)
            Array.Reverse(bitssx);
        Single result = 0;
        result = BitConverter.ToSingle(bitssx, 0);
        strean.Flush();
        return result;
    }
    /// <summary>
    /// Return size in human readable form
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <param name="useUnit ">Includes measure unit (default: false)</param>
    /// <returns>Readable value</returns>
    public static string FormatBytes(this long bytes, bool useUnit = false)
    {
        string[] Suffix = { " B", " kB", " MB", " GB", " TB" };
        double dblSByte = bytes;
        int i;
        for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;
        }
        return $"{dblSByte:0.##}{(useUnit ? Suffix[i] : null)}";
    }
    public static void CreateInfo(this string[] data, Stream xml, bool close = false)
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("RAIDEN_PATCHER_INFO");

        #region Info Childs
        #region Info
        XmlElement info = doc.CreateElement("Info");

        //MD5 Hash
        var md5 = doc.CreateElement("MD5");
        md5.AppendChild(doc.CreateTextNode(data[0]));

        //Info
        var resumo = doc.CreateElement("Resumo");
        resumo.AppendChild(doc.CreateTextNode(data[1]));

        //Equipe
        var team = doc.CreateElement("Equipe");
        team.AppendChild(doc.CreateTextNode(data[2]));

        //Nome de Volume
        var volume = doc.CreateElement("Volume");
        volume.AppendChild(doc.CreateTextNode(data[3]));

        //Files Deleted in ISO
        var iso = doc.CreateElement("ISO");
        iso.AppendChild(doc.CreateTextNode(data[4]));

        info.AppendChild(md5);
        info.AppendChild(resumo);
        info.AppendChild(team);
        info.AppendChild(volume);
        info.AppendChild(iso);
        #endregion

        root.AppendChild(info);
        #endregion

        doc.AppendChild(root);
        doc.Save(xml);
        if (close)
            xml.Close();
    }

    public static string[] LoadInfos(this StreamReader xml)
    {
        var outStrs = new List<string>();
        XmlDocument reader = new XmlDocument();
        reader.CreateElement("root");
        reader.Load(xml);

        #region Common Tags
        XmlNodeList xnList = reader.GetElementsByTagName("Info");
        foreach (XmlNode node in xnList)
            foreach (XmlNode child in node.ChildNodes)
                switch (child.Name)
                {
                    case "MD5":
                        outStrs.Add(child.InnerText);
                        break;
                    case "Resumo":
                        outStrs.Add(child.InnerText);
                        break;
                    case "Equipe":
                        outStrs.Add(child.InnerText);
                        break;
                    case "Volume":
                        outStrs.Add(child.InnerText);
                        break;
                    case "ISO":
                        outStrs.Add(child.InnerText);
                        break;
                }
        #endregion

        return outStrs.ToArray();
    }

    public static bool StreamsContentsAreEqual(Stream stream1, Stream stream2)
    {
        const int bufferSize = 1024 * sizeof(Int64);
        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            int count1 = stream1.Read(buffer1, 0, bufferSize);
            int count2 = stream2.Read(buffer2, 0, bufferSize);

            if (count1 != count2)
            {
                return false;
            }

            if (count1 == 0)
            {
                return true;
            }

            int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
            for (int i = 0; i < iterations; i++)
            {
                if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                {
                    return false;
                }
            }
        }
    }

    #region Leitores de Dados

    #region BITS
    /// <summary>
    /// Lê um bit de um determinado array[byte].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura(0-7).</param>
    /// <returns>Bool.</returns>
    public static bool ReadBit(this byte array, int offset)
    {
        BitArray arra = new BitArray(new byte[] { array });
        return arra[offset];
    }

    /// <summary>
    /// Lê todos os bits de um determinado array[byte].
    /// </summary>
    /// <returns>BitArray.</returns>
    public static BitArray ReadBits(this byte array)
    {
        BitArray arra = new BitArray(new byte[] { array });
        return arra;
    }

    /// <summary>
    /// Converte todos os bits do array em uma string.
    /// </summary>
    /// <returns>string.</returns>
    public static string ToSTR(this BitArray array)
    {
        string result = "";
        foreach (var inte in array)
            result += Convert.ToInt32(inte).ToString();
        // fix: result é string, .Reverse() aqui é LINQ sobre char — forçar ToCharArray
        char[] chars = result.ToCharArray();
        Array.Reverse(chars);
        result = new string(chars);
        return result;
    }
    #endregion

    #region BYTES
    /// <summary>
    /// Lê uma quantia específica de bytes de um determinado array[buffer].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this byte[] array, int offset, int size)
    {
        byte[] result = array.Skip(offset).Take(size).ToArray();
        return result;
    }

    /// <summary>
    /// Lê uma quantia específica de bytes de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this Stream stream, int offset, int size)
    {
        var result = new List<byte>();
        stream.Position = (long)offset;
        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte());
        return result.ToArray();
    }

    /// <summary>
    /// Lê uma quantia específica de bytes de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytesLong(this Stream stream, long offset, int size)
    {
        var result = new List<byte>();
        stream.Position = offset;
        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte());
        return result.ToArray();
    }

    /// <summary>
    /// Lê uma seção de arquivos de um fluxo e posição determinado[Stream].
    /// </summary>
    /// <param name="lba">Posição LBA para iniciar a leitura.</param>
    /// <param name="size">Tamanho dos setores.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadFiles(this Stream stream, int lba, int size = 2048)
    {
        var result = new List<byte>();
        int offset = lba * size;
        bool stop = false;
        while (stop == false)
        {
            stream.Position = offset;
            byte sizex = (byte)stream.ReadByte();
            if (sizex == 0)
                stop = true;
            if (stop != true)
            {
                stream.Position = offset;
                byte[] section = stream.ReadBytes(offset, sizex);
                result.AddRange(section);
                offset += section.Length;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Lê um setor de um array de bytes[buffer].
    /// </summary>
    /// <param name="lba">Localização de bloco lógico.[LBA]</param>
    /// <param name="size">Tamanho do setor para leitura.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadSector(this byte[] array, int lba, int size = 2048)
    {
        byte[] result = array.ReadBytes(lba * size, size);
        return result;
    }

    /// <summary>
    /// Lê um setor de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="lba">Localização de bloco lógico.[LBA]</param>
    /// <param name="size">Tamanho do setor para leitura.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadSector(this Stream stream, int lba, int size = 2048)
    {
        byte[] result = stream.ReadBytes(lba * size, size);
        return result;
    }
    #endregion

    #endregion

    #region Escrita de Dados

    /// <summary>
    /// Escreve uma quantia específica de bytes em um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="offsetwrite">Posição para iniciar a escrita.</param>
    /// <param name="size">Tamanho da escrita em bytes.</param>
    public static void WriteBytes(this Stream stream, Stream write, long offsetwrite, long offset, long size)
    {
        stream.Position = offsetwrite;
        write.Position = offset;
        int c = 0;
        while (c < size)
        {
            stream.WriteByte((byte)write.ReadByte());
            c++;
        }
    }

    #endregion

    #region Leitores de Inteiros
    /// <summary>
    /// Lê um Inteiro com sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Inteiro com sinal(int)</returns>
    public static int ReadInt(this byte[] array, int offset, int bits, bool bigendian = false)
    {
        var reader = new BinaryReader(new MemoryStream(array));
        reader.BaseStream.Position = offset;
        int result = 0;
        switch (bits)
        {
            case 8:
                result = (sbyte)reader.ReadByte();
                break;
            case 16:
                result = reader.ReadInt16();
                break;
            case 32:
                result = reader.ReadInt32();
                break;
        }
        reader.Close();
        byte[] bytes = BitConverter.GetBytes(result);
        if (bigendian) Array.Reverse(bytes);
        result = BitConverter.ToInt32(bytes, 0);
        return result;
    }

    /// <summary>
    /// Lê um Inteiro sem sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this byte[] array, int offset, int bits, bool bigendian = false)
    {
        var reader = new BinaryReader(new MemoryStream(array));
        reader.BaseStream.Position = offset;
        uint result = 0;
        switch (bits)
        {
            case 8:
                result = (uint)reader.ReadByte();
                break;
            case 16:
                result = reader.ReadUInt16();
                break;
            case 32:
                result = reader.ReadUInt32();
                break;
        }
        reader.Close();
        byte[] bytes = BitConverter.GetBytes(result);
        if (bigendian) Array.Reverse(bytes);
        result = BitConverter.ToUInt32(bytes, 0);
        return result;
    }

    /// <summary>
    /// Lê um Inteiro sem sinal do fluxo[Stream].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this Stream strean, int offset, int bits, bool bigendian = false)
    {
        byte[] bitssx = strean.ReadBytes(offset, (int)(bits / 8));
        uint result = 0;
        switch (bits)
        {
            case 8:
                result = bitssx[0];
                break;
            case 16:
                {
                    byte[] b = (byte[])bitssx.Clone();
                    if (bigendian) Array.Reverse(b);
                    result = BitConverter.ToUInt16(b, 0);
                    break;
                }
            case 32:
                {
                    byte[] b = (byte[])bitssx.Clone();
                    if (bigendian) Array.Reverse(b);
                    result = BitConverter.ToUInt32(b, 0);
                    break;
                }
        }
        return result;
    }

    /// <summary>
    /// Lê um Long sem sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Long sem sinal(ulong)</returns>
    public static ulong ReadULong(this byte[] array, int offset, bool bigendian = false)
    {
        byte[] b = array.ReadBytes(offset, 8);
        if (bigendian) Array.Reverse(b);
        return BitConverter.ToUInt64(b, 0);
    }

    /// <summary>
    /// Lê um Long sem sinal do fluxo[Stream].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Long sem sinal(ulong)</returns>
    public static ulong ReadULong(this Stream strean, int offset, bool bigendian = false)
    {
        byte[] b = strean.ReadBytes(offset, 8);
        if (bigendian) Array.Reverse(b);
        return BitConverter.ToUInt64(b, 0);
    }
    #endregion

    #region Leitores de Texto

    /// <summary>
    /// Lê um array de bytes enquanto diferente de uma quebra no array(algum byte)[buffer].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>byte[]</returns>
    public static byte[] ReadBroke(this byte[] file, int offset, byte breakeroff = 0)
    {
        byte[] result = file.Skip(offset).TakeWhile(x => x != breakeroff).ToArray();
        return result;
    }

    /// <summary>
    /// Lê um array de bytes enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>byte[]</returns>
    public static byte[] ReadBroke(this Stream file, int offset, byte breakeroff = 0)
    {
        var result = new List<byte>();
        file.Position = offset;
        while (file.ReadBytes((int)file.Position, 1)[0] != breakeroff)
        {
            result.Add((byte)file.ReadByte());
        }
        return result.ToArray();
    }

    //Stream
    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="encoding">Codificação de leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this Stream file, int offset, Encoding encoding, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return encoding.GetString(traw);
    }

    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this Stream file, int offset, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return Encoding.Default.GetString(traw);
    }

    //Array
    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Array].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="encoding">Codificação de leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this byte[] file, int offset, Encoding encoding, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return encoding.GetString(traw);
    }

    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Array].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this byte[] file, int offset, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return Encoding.Default.GetString(traw);
    }

    //Conversores
    /// <summary>
    /// Converte uma string para uma codificação específica[string].
    /// </summary>
    /// <param name="encoding">Codificação de saída.</param>
    /// <returns>string</returns>
    public static string ConvertTo(this string file, Encoding encoding)
    {
        return encoding.GetString(encoding.GetBytes(file));
    }

    /// <summary>
    /// Converte uma string para uma codificação específica[string].
    /// </summary>
    /// <param name="encoding">Codificação de saída.</param>
    /// <returns>string</returns>
    public static string ConvertTo(this byte[] file, Encoding encoding)
    {
        return encoding.GetString(file);
    }
    #endregion

    #region Extra

    public static byte[] GetFilledString(this string str, int size, byte fillwith)
    {
        byte[] outbin = new byte[size];
        outbin.FillArray(fillwith);
        Encoding.Default.GetBytes(str).CopyTo(outbin, 0);
        return outbin;
    }
    public static byte[] FillArray(this byte[] array, byte value)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] != value)
                array[i] = value;
        return array;
    }
    public static byte ToByte(this BitArray array)
    {
        byte[] outbin = new byte[1];
        array.CopyTo(outbin, 0);
        return outbin[0];
    }
    public static byte[] ToLEBE(this uint entry, int bits)
    {
        var outbin = new List<byte>();
        if (bits == 16)
        {
            byte[] le = BitConverter.GetBytes((UInt16)entry);
            byte[] be = (byte[])le.Clone(); Array.Reverse(be);
            outbin.AddRange(le);
            outbin.AddRange(be);
        }
        else if (bits == 64)
        {
            byte[] le = BitConverter.GetBytes((UInt64)entry);
            byte[] be = (byte[])le.Clone(); Array.Reverse(be);
            outbin.AddRange(le);
            outbin.AddRange(be);
        }
        else if (bits == 32)
        {
            byte[] le = BitConverter.GetBytes((UInt32)entry);
            byte[] be = (byte[])le.Clone(); Array.Reverse(be);
            outbin.AddRange(le);
            outbin.AddRange(be);
        }
        return outbin.ToArray();
    }
    #endregion
}