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
public class Desconhecido: Setor
{
    public Desconhecido(int lba, int tamanho)
    {
        this.lba = lba;
        this.tamanhosetor = tamanho;
        this.offsetsetor = lba * tamanho;
    }
}

