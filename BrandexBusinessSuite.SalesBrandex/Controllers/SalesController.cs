namespace BrandexBusinessSuite.SalesBrandex.Controllers;

using System.Text;
using System.Collections.Immutable;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Services;
using BrandexBusinessSuite.Controllers;
using Models.ErpDocuments;

using static  Common.Constants;
using static Common.ErpConstants;
using static Requests.RequestsMethods;

public class SalesController : ApiController
{
    private readonly ErpUserSettings _erpUserSettings;
    private static readonly HttpClient Client = new();
    
    public SalesController(IOptions<ErpUserSettings> erpUserSettings)
    {
        _erpUserSettings = erpUserSettings.Value;
    }
    
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> GetSales()
    {
        // var productsDb = await _productsService.GetCheckModels();
        
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await JObjectByUriGetRequest(Client,
            $"{ErpRequests.BaseUrl}Crm_Sales_SalesOrders?$top=10000&$filter=CreationTime%20ge%202022-10-01T00:00:00.000Z%20and%20CreationTime%20le%202022-10-31T00:00:00.000Z&$select=DocumentDate,Id,ShipToCustomer&$expand=Lines($expand=Product($select=Id,Name);$select=Id,LineAmount,Product,Quantity),ShipToCustomer($expand=Party($select=CustomProperty_RETREG,PartyCode,PartyName);$select=CustomProperty_GRAD_u002DKLIENT,CustomProperty_Klas_u0020Klient,CustomProperty_STOR3,Id),ShipToPartyContactMechanism($expand=ContactMechanism;$select=ContactMechanism),ToParty($select=PartyName)");
        var batchesList = JsonConvert.DeserializeObject<List<ErpSalesOrderAnalysis>>(responseContentJObj["value"].ToString());
        
        return Result.Success;

    }

    private static readonly ImmutableDictionary<char, string> cyrillicToLatinMapping = new Dictionary<char, string>
    {
        { 'а', "a"}, { 'А', "A"},
        { 'б', "b"}, { 'Б', "B"},
        { 'в', "v"}, { 'В', "V"},
        { 'г', "g"}, { 'Г', "G"},
        { 'д', "d"}, { 'Д', "D"},
        { 'е', "e"}, { 'Е', "E"},
        { 'ж', "zh"}, { 'Ж', "Zh"},
        { 'з', "z"}, { 'З', "Z"},
        { 'и', "i"}, { 'И', "I"},
        { 'й', "y"}, { 'Й', "Y"},
        { 'к', "k"}, { 'К', "K"},
        { 'л', "l"}, { 'Л', "L"},
        { 'м', "m"}, { 'М', "M"},
        { 'н', "n"}, { 'Н', "N"},
        { 'о', "o"}, { 'О', "O"},
        { 'п', "p"}, { 'П', "P"},
        { 'р', "r"}, { 'Р', "R"},
        { 'с', "s"}, { 'С', "S"},
        { 'т', "t"}, { 'Т', "T"},
        { 'у', "u"}, { 'У', "U"},
        { 'ф', "f"}, { 'Ф', "F"},
        { 'х', "h"}, { 'Х', "H"},
        { 'ц', "ts"}, { 'Ц', "Ts"},
        { 'ч', "ch"}, { 'Ч', "Ch"},
        { 'ш', "sh"}, { 'Ш', "Sh"},
        { 'щ', "sht"}, { 'Щ', "Sht"},
        { 'ъ', "a"}, { 'Ъ', "A"},
        { 'ь', "y"}, { 'Ь', "Y"},
        { 'ю', "yu"}, { 'Ю', "Yu"},
        { 'я', "ya"}, { 'Я', "Ya"},
        { ' ', " " }
    }.ToImmutableDictionary();

    private static readonly ImmutableArray<(string Latin, char Cyrillic)> latinToCyrillicMapping =
        cyrillicToLatinMapping
            .OrderByDescending(v => v.Value.Length)
            .Select(d => (Latin: d.Value, Cyrillic: d.Key))
            .ToImmutableArray();

    public static string CyrillicToLatin(string text)
        => string.Join("", text.ToCharArray().Select(c => cyrillicToLatinMapping[c]));

    public static string LatinToCyrillic(string text)
    {
        int startIdx = 0;
        StringBuilder accumulator = new();
        while (startIdx != text.Length)
        {
            foreach (var (latin, cyrillic) in latinToCyrillicMapping)
            {
                if (text[startIdx..].StartsWith(latin))
                {
                    accumulator.Append(cyrillic);
                    startIdx += latin.Length;
                    break;
                }
            }
        }
        return accumulator.ToString();
    }
}