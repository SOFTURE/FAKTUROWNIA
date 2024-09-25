
using System.Text.Json.Serialization;
using SOFTURE.Fakturownia.Models.Enums;

namespace SOFTURE.Fakturownia.Models;

public sealed class CalculatingStrategy
{
    [JsonPropertyName("position")] public string? Position { get; set; }

    [JsonPropertyName("sum")] public string? Sum { get; set; }

    [JsonPropertyName("invoice_form_price_kind")] public string? InvoiceFormPriceKind { get; set; }
}

public sealed class InvoiceDetails
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("user_id")] public int UserId { get; set; }

    [JsonPropertyName("app")] public object? App { get; set; }

    [JsonPropertyName("number")] public string? Number { get; set; }

    [JsonPropertyName("place")] public string? Place { get; set; }

    [JsonPropertyName("sell_date")] public string? SellDate { get; set; }

    [JsonPropertyName("payment_type")] public string? PaymentType { get; set; }

    [JsonPropertyName("price_net")] public string PriceNet { get; set; } = null!;

    [JsonPropertyName("price_gross")] public string? PriceGross { get; set; }

    [JsonPropertyName("currency")] public string? Currency { get; set; }

    [JsonPropertyName("status")] public string? Status { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("seller_name")] public string? SellerName { get; set; }

    [JsonPropertyName("seller_tax_no")] public string? SellerTaxNo { get; set; }

    [JsonPropertyName("seller_street")] public string? SellerStreet { get; set; }

    [JsonPropertyName("seller_post_code")] public string? SellerPostCode { get; set; }

    [JsonPropertyName("seller_city")] public string? SellerCity { get; set; }

    [JsonPropertyName("seller_country")] public string? SellerCountry { get; set; }

    [JsonPropertyName("seller_email")] public string? SellerEmail { get; set; }

    [JsonPropertyName("seller_phone")] public string? SellerPhone { get; set; }

    [JsonPropertyName("seller_fax")] public string? SellerFax { get; set; }

    [JsonPropertyName("seller_www")] public string? SellerWww { get; set; }

    [JsonPropertyName("seller_person")] public string? SellerPerson { get; set; }

    [JsonPropertyName("seller_bank")] public string? SellerBank { get; set; }

    [JsonPropertyName("seller_bank_account")] public string? SellerBankAccount { get; set; }

    [JsonPropertyName("buyer_name")] public string? BuyerName { get; set; }

    [JsonPropertyName("buyer_tax_no")] public string? BuyerTaxNo { get; set; }

    [JsonPropertyName("buyer_post_code")] public string? BuyerPostCode { get; set; }

    [JsonPropertyName("buyer_city")] public string? BuyerCity { get; set; }

    [JsonPropertyName("buyer_street")] public string? BuyerStreet { get; set; }

    [JsonPropertyName("buyer_first_name")] public string? BuyerFirstName { get; set; }

    [JsonPropertyName("buyer_country")] public string? BuyerCountry { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")] public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("token")] public string? Token { get; set; }

    [JsonPropertyName("buyer_email")] public string? BuyerEmail { get; set; }

    [JsonPropertyName("buyer_www")] public string? BuyerWww { get; set; }

    [JsonPropertyName("buyer_fax")] public string? BuyerFax { get; set; }

    [JsonPropertyName("buyer_phone")] public string? BuyerPhone { get; set; }

    [JsonPropertyName("kind")] public DocumentKind Kind { get; set; }

    [JsonPropertyName("pattern")] public string? Pattern { get; set; }

    [JsonPropertyName("pattern_nr")] public object? PatternNr { get; set; }

    [JsonPropertyName("pattern_nr_m")] public int? PatternNrM { get; set; }

    [JsonPropertyName("pattern_nr_d")] public object? PatternNrD { get; set; }

    [JsonPropertyName("client_id")] public int ClientId { get; set; }

    [JsonPropertyName("payment_to")] public string? PaymentTo { get; set; }

    [JsonPropertyName("paid")] public string? Paid { get; set; }

    [JsonPropertyName("seller_bank_account_id")] public object? SellerBankAccountId { get; set; }

    [JsonPropertyName("lang")] public string? Lang { get; set; }

    [JsonPropertyName("issue_date")] public string? IssueDate { get; set; }

    [JsonPropertyName("price_tax")] public string? PriceTax { get; set; }

    [JsonPropertyName("department_id")] public int? DepartmentId { get; set; }

    [JsonPropertyName("correction")] public object? Correction { get; set; }

    [JsonPropertyName("buyer_note")] public string? BuyerNote { get; set; }

    [JsonPropertyName("additional_info_desc")] public string? AdditionalInfoDesc { get; set; }

    [JsonPropertyName("additional_info")] public bool? AdditionalInfo { get; set; }

    [JsonPropertyName("product_cache")] public string? ProductCache { get; set; }

    [JsonPropertyName("buyer_last_name")] public string? BuyerLastName { get; set; }

    [JsonPropertyName("from_invoice_id")] public int? FromInvoiceId { get; set; }

    [JsonPropertyName("oid")] public string? Oid { get; set; }

    [JsonPropertyName("discount")] public string? Discount { get; set; }

    [JsonPropertyName("show_discount")] public bool? ShowDiscount { get; set; }

    [JsonPropertyName("sent_time")] public object? SentTime { get; set; }

    [JsonPropertyName("print_time")] public object? PrintTime { get; set; }

    [JsonPropertyName("recurring_id")] public object? RecurringId { get; set; }

    [JsonPropertyName("tax2_visible")] public object? Tax2Visible { get; set; }

    [JsonPropertyName("warehouse_id")] public object? WarehouseId { get; set; }

    [JsonPropertyName("paid_date")] public string? PaidDate { get; set; }

    [JsonPropertyName("product_id")] public int? ProductId { get; set; }

    [JsonPropertyName("issue_year")] public int? IssueYear { get; set; }

    [JsonPropertyName("internal_note")] public string? InternalNote { get; set; }

    [JsonPropertyName("invoice_id")] public object? InvoiceId { get; set; }

    [JsonPropertyName("invoice_template_id")] public int? InvoiceTemplateId { get; set; }

    [JsonPropertyName("description_long")] public string? DescriptionLong { get; set; }

    [JsonPropertyName("buyer_tax_no_kind")] public object? BuyerTaxNoKind { get; set; }

    [JsonPropertyName("seller_tax_no_kind")] public object? SellerTaxNoKind { get; set; }

    [JsonPropertyName("description_footer")] public string? DescriptionFooter { get; set; }

    [JsonPropertyName("sell_date_kind")] public string? SellDateKind { get; set; }

    [JsonPropertyName("payment_to_kind")] public string? PaymentToKind { get; set; }

    [JsonPropertyName("exchange_currency")] public object? ExchangeCurrency { get; set; }

    [JsonPropertyName("discount_kind")] public string? DiscountKind { get; set; }

    [JsonPropertyName("income")] public bool? Income { get; set; }

    [JsonPropertyName("from_api")] public bool? FromApi { get; set; }

    [JsonPropertyName("category_id")] public object? CategoryId { get; set; }

    [JsonPropertyName("warehouse_document_id")] public object? WarehouseDocumentId { get; set; }

    [JsonPropertyName("exchange_kind")] public string? ExchangeKind { get; set; }

    [JsonPropertyName("exchange_rate")] public string? ExchangeRate { get; set; }

    [JsonPropertyName("use_delivery_address")] public bool UseDeliveryAddress { get; set; }

    [JsonPropertyName("delivery_address")] public string? DeliveryAddress { get; set; }

    [JsonPropertyName("accounting_kind")] public object? AccountingKind { get; set; }

    [JsonPropertyName("buyer_person")] public string? BuyerPerson { get; set; }

    [JsonPropertyName("buyer_bank_account")] public string? BuyerBankAccount { get; set; }

    [JsonPropertyName("buyer_bank")] public string? BuyerBank { get; set; }

    [JsonPropertyName("buyer_mass_payment_code")] public object? BuyerMassPaymentCode { get; set; }

    [JsonPropertyName("exchange_note")] public string? ExchangeNote { get; set; }

    [JsonPropertyName("buyer_company")] public bool? BuyerCompany { get; set; }

    [JsonPropertyName("show_attachments")] public bool? ShowAttachments { get; set; }

    [JsonPropertyName("exchange_currency_rate")] public object? ExchangeCurrencyRate { get; set; }

    [JsonPropertyName("has_attachments")] public bool? HasAttachments { get; set; }

    [JsonPropertyName("exchange_date")] public object? ExchangeDate { get; set; }

    [JsonPropertyName("attachments_count")] public int AttachmentsCount { get; set; }

    [JsonPropertyName("delivery_date")] public string? DeliveryDate { get; set; }

    [JsonPropertyName("fiscal_status")] public object? FiscalStatus { get; set; }

    [JsonPropertyName("use_moss")] public bool UseMoss { get; set; }

    [JsonPropertyName("calculating_strategy")] public CalculatingStrategy? CalculatingStrategy { get; set; }

    [JsonPropertyName("transaction_date")] public string? TransactionDate { get; set; }

    [JsonPropertyName("email_status")] public object? EmailStatus { get; set; }

    [JsonPropertyName("exclude_from_stock_level")] public bool ExcludeFromStockLevel { get; set; }

    [JsonPropertyName("exclude_from_accounting")] public bool ExcludeFromAccounting { get; set; }

    [JsonPropertyName("exchange_rate_den")] public string? ExchangeRateDen { get; set; }

    [JsonPropertyName("exchange_currency_rate_den")] public string? ExchangeCurrencyRateDen { get; set; }

    [JsonPropertyName("accounting_scheme")] public object? AccountingScheme { get; set; }

    [JsonPropertyName("exchange_difference")] public string? ExchangeDifference { get; set; }

    [JsonPropertyName("not_cost")] public bool NotCost { get; set; }

    [JsonPropertyName("reverse_charge")] public bool ReverseCharge { get; set; }

    [JsonPropertyName("issuer")] public object? Issuer { get; set; }

    [JsonPropertyName("use_issuer")] public bool UseIssuer { get; set; }

    [JsonPropertyName("cancelled")] public bool Cancelled { get; set; }

    [JsonPropertyName("recipient_id")] public object? RecipientId { get; set; }

    [JsonPropertyName("recipient_name")] public string? RecipientName { get; set; }

    [JsonPropertyName("test")] public bool Test { get; set; }

    [JsonPropertyName("discount_net")] public object? DiscountNet { get; set; }

    [JsonPropertyName("approval_status")] public object? ApprovalStatus { get; set; }

    [JsonPropertyName("accounting_vat_tax_date")] public string? AccountingVatTaxDate { get; set; }

    [JsonPropertyName("accounting_income_tax_date")] public string? AccountingIncomeTaxDate { get; set; }

    [JsonPropertyName("accounting_other_tax_date")] public object? AccountingOtherTaxDate { get; set; }

    [JsonPropertyName("accounting_status")] public object? AccountingStatus { get; set; }

    [JsonPropertyName("normalized_number")] public object? NormalizedNumber { get; set; }

    [JsonPropertyName("na_tax_kind")] public object? NaTaxKind { get; set; }

    [JsonPropertyName("issued_to_receipt")] public bool? IssuedToReceipt { get; set; }

    [JsonPropertyName("gov_id")] public object? GovId { get; set; }

    [JsonPropertyName("gov_kind")] public object? GovKind { get; set; }

    [JsonPropertyName("gov_status")] public object? GovStatus { get; set; }

    [JsonPropertyName("sales_code")] public string? SalesCode { get; set; }

    [JsonPropertyName("additional_invoice_field")] public object? AdditionalInvoiceField { get; set; }

    [JsonPropertyName("products_margin")] public object? ProductsMargin { get; set; }

    [JsonPropertyName("payment_url")] public object? PaymentUrl { get; set; }

    [JsonPropertyName("view_url")] public string? ViewUrl { get; set; }

    [JsonPropertyName("buyer_mobile_phone")] public string? BuyerMobilePhone { get; set; }

    [JsonPropertyName("kind_text")] public string? KindText { get; set; }

    [JsonPropertyName("invoice_for_receipt_id")] public object? InvoiceForReceiptId { get; set; }

    [JsonPropertyName("receipt_for_invoice_id")] public object? ReceiptForInvoiceId { get; set; }

    [JsonPropertyName("recipient_company")] public string? RecipientCompany { get; set; }

    [JsonPropertyName("recipient_first_name")] public string? RecipientFirstName { get; set; }

    [JsonPropertyName("recipient_last_name")] public string? RecipientLastName { get; set; }

    [JsonPropertyName("recipient_tax_no")] public string? RecipientTaxNo { get; set; }

    [JsonPropertyName("recipient_street")] public string? RecipientStreet { get; set; }

    [JsonPropertyName("recipient_post_code")] public string? RecipientPostCode { get; set; }

    [JsonPropertyName("recipient_city")] public string? RecipientCity { get; set; }

    [JsonPropertyName("recipient_country")] public string? RecipientCountry { get; set; }

    [JsonPropertyName("recipient_email")] public string? RecipientEmail { get; set; }

    [JsonPropertyName("recipient_phone")] public string? RecipientPhone { get; set; }

    [JsonPropertyName("recipient_note")] public string? RecipientNote { get; set; }

    [JsonPropertyName("overdue?")] public bool? Overdue { get; set; }

    [JsonPropertyName("get_tax_name")] public string? GetTaxName { get; set; }

    [JsonPropertyName("tax_visible?")] public bool? TaxVisible { get; set; }

    [JsonPropertyName("tax_name_type")] public string? TaxNameType { get; set; }

    [JsonPropertyName("split_payment")] public string? SplitPayment { get; set; }

    [JsonPropertyName("gtu_codes")] public List<object>? GtuCodes { get; set; }

    [JsonPropertyName("procedure_designations")] public List<object>? ProcedureDesignations { get; set; }
}