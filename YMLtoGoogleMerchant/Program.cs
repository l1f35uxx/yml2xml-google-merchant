using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        string ymlUrl = "https://sewberi.ru/bitrix/catalog_export/ya_webmaster_1601231409.php";  // Указанная ссылка
        string outputFilePath = "google_merchant.xml";  // Путь для сохранения XML файла

        try
        {
            // Шаг 1: Скачать YML файл по ссылке
            var ymlContent = await DownloadYmlFileAsync(ymlUrl);
            Console.WriteLine("YML файл успешно скачан.");

            // Шаг 2: Преобразовать YML в XML для Google Merchant
            XDocument googleMerchantXml = ConvertYmlToXml(ymlContent);
            Console.WriteLine("YML успешно преобразован в XML для Google Merchant.");

            // Шаг 3: Сохранить XML в файл
            googleMerchantXml.Save(outputFilePath);
            Console.WriteLine($"Файл XML сохранен по пути: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Метод для скачивания YML файла
    static async Task<string> DownloadYmlFileAsync(string url)
    {
        using (var client = new HttpClient())
        {
            // Скачать YML файл как строку
            var response = await client.GetStringAsync(url);
            return response;
        }
    }

    // Метод для преобразования YML в XML
    static XDocument ConvertYmlToXml(string ymlContent)
    {
        // Загружаем YML как XML документ
        XDocument ymlXml = XDocument.Parse(ymlContent);

        // Создаем новый XML документ для Google Merchant
        XElement googleMerchantXml = new XElement("rss", new XAttribute("version", "2.0"),
            new XElement("channel",
                new XElement("title", "Магазин товаров"),
                new XElement("link", "https://example.com"),
                new XElement("description", "Описание магазина"),
                new XElement("language", "ru")
            )
        );

        // Перебираем товары из YML и добавляем их в XML для Google Merchant
        foreach (var offer in ymlXml.Descendants("offer"))
        {
            var item = new XElement("item",
                new XElement("g:id", offer.Element("id")?.Value),
                new XElement("g:title", offer.Element("name")?.Value),
                new XElement("g:description", offer.Element("description")?.Value),
                new XElement("g:link", offer.Element("url")?.Value),
                new XElement("g:image_link", offer.Element("picture")?.Value),
                new XElement("g:price", offer.Element("price")?.Value),
                new XElement("g:brand", offer.Element("vendor")?.Value),
                new XElement("g:availability", offer.Element("available")?.Value == "true" ? "in_stock" : "out_of_stock")
            );

            googleMerchantXml.Element("channel").Add(item);
        }

        // Возвращаем готовый XML документ
        return new XDocument(googleMerchantXml);
    }
}
