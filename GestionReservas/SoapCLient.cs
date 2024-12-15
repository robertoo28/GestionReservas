using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Xml;

public class SoapClient
{
    private readonly HttpClient _httpClient;

    public SoapClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        _httpClient = new HttpClient(handler);
        _httpClient.BaseAddress = new Uri("https://localhost:7259/AvailabilityService.asmx");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
    }

    public async Task<bool> CheckAvailability(DateTime startDate, DateTime endDate, string roomType)
    {
        // Construir solicitud SOAP
        var soapRequest = $@"
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body>
                    <CheckAvailability xmlns=""http://tempuri.org/"">
                        <startDate>{startDate:yyyy-MM-dd}</startDate>
                        <endDate>{endDate:yyyy-MM-dd}</endDate>
                        <roomType>{roomType}</roomType>
                    </CheckAvailability>
                </soap:Body>
            </soap:Envelope>";

        var content = new StringContent(soapRequest, System.Text.Encoding.UTF8, "text/xml");

        // Enviar solicitud
        var response = await _httpClient.PostAsync(string.Empty, content);

        response.EnsureSuccessStatusCode(); // Lanza excepción si el código no es exitoso (200-299)

        // Procesar respuesta (simplificado)
        var responseContent = await response.Content.ReadAsStringAsync();
        var isAvailable = ParseAvailabilityResponse(responseContent);

        return isAvailable;
    }
    
    private bool ParseAvailabilityResponse(string soapResponse)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            // Navegar al nodo CheckAvailabilityResult/Rooms
            var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaceManager.AddNamespace("t", "http://tempuri.org/");

            var roomsNode = xmlDoc.SelectSingleNode("//t:CheckAvailabilityResult/Rooms", namespaceManager);

            // Si el nodo Rooms está vacío, no hay disponibilidad
            return !(roomsNode != null && !string.IsNullOrEmpty(roomsNode.InnerXml));
        }
        catch (Exception ex)
        {
            throw new Exception("Error al procesar la respuesta del servicio SOAP.", ex);
        }
    }
}