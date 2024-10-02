using System.Runtime.Serialization;

namespace SOFTURE.Fakturownia.Models.Api.Enums;

public enum DocumentKind
{
    [EnumMember(Value = "vat")]
    Vat,
    
    [EnumMember(Value = "proforma")]
    Proforma,
    
    [EnumMember(Value = "bill")]
    Bill,
    
    [EnumMember(Value = "receipt")]
    Receipt,
    
    [EnumMember(Value = "advance")]
    Advance,
    
    [EnumMember(Value = "correction")]
    Correction,
    
    [EnumMember(Value = "vat_mp")]
    VatMp,
    
    [EnumMember(Value = "invoice_other")]
    InvoiceOther,
    
    [EnumMember(Value = "vat_margin")]
    VatMargin,
    
    [EnumMember(Value = "kp")]
    Kp,
    
    [EnumMember(Value = "kw")]
    Kw,
    
    [EnumMember(Value = "final")]
    Final,
    
    [EnumMember(Value = "estimate")]
    Estimate,
    
    [EnumMember(Value = "accounting_note")]
    AccountingNote
}