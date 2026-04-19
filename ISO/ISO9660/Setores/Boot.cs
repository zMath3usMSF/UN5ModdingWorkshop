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
public class Boot: Setor
{
    public string SystemBootID;
    public string SystemID;
    
    public byte[] BootData
    {
        get => iso.ReadBytes(offsetsetor + 0x47, 0x7b9);
    }
    public Boot(Stream reader,int lba, int tamanho)
    {
        this.lba = lba;
        this.tipo = Tipo_de_Descritor.BootRecord;
        this.tamanhosetor = tamanho;
        this.offsetsetor = lba * tamanho;
        this.NomeSeção = reader.ReadBytes(offsetsetor+1, 5).ConvertTo(Encoding.Default);
        this.Versão = reader.ReadBytes(offsetsetor+6, 1)[0];

        SystemBootID = reader.ReadBytes(offsetsetor + 7, 0x20).ConvertTo(Encoding.Default);
        SystemID = reader.ReadBytes(offsetsetor + 27, 0x20).ConvertTo(Encoding.Default);
    }
}

