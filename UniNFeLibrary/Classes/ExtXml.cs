﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UniNFeLibrary
{
    public class ExtXml
    {
        public static string Nfe = "-nfe.xml";
        public const string EnvLot = "-env-lot.xml";
        public const string PedRec = "-ped-rec.xml";
        public const string PedSit = "-ped-sit.xml";
        public const string PedInu = "-ped-inu.xml";
        public const string PedCan = "-ped-can.xml";
        public const string PedSta = "-ped-sta.xml";
        public static string ProcNFe = "-procNFe.xml";
        public static string ProcCancNFe = "-procCancNFe.xml";
        public static string ProcInutNFe = "-procInutNFe.xml";
        public const string ConsCad = "-cons-cad.xml";
        public const string ConsInf = "-cons-inf.xml";
        public const string AltCon = "-alt-con.xml";
        public const string MontarLote = "-montar-lote.xml";
    }

    /// <summary>
    /// Tipo de emissão da NFe - danasa 8-2009
    /// </summary>
    public class TipoEmissao
    {
        public const int teNormal = 1;
        public const int teContingencia = 2;
        public const int teSCAN = 3;
        public const int teDPEC = 4;
        public const int teFSDA = 5;
    }

    /// <summary>
    /// Tipos de ambientes da NFe - danasa 8-2009
    /// </summary>
    public class TipoAmbiente
    {
        public const int taProducao = 1;
        public const int taHomologacao = 2;
    }

    /// <summary>
    /// Parâmetros necessários para o envio dos XML´s
    /// </summary>
    public class ParametroEnvioXML
    {
        public int tpAmb { get; set; }
        public int tpEmis { get; set; }
        public int UFCod { get; set; }
    }
}
