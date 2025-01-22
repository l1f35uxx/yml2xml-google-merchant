using System;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;

class Program
{
    static async Task Main(string[] args)
    {
        // URL YML файла
        string ymlUrl = "https://example.com/products.yml"; // Замените на свой URL

        try
        {
            // Загружаем YML файл
            string ymlContent = await DownloadYML(ymlUrl);

            // Парсим YML в объект
            var products = ParseYML(ymlContent);

            // Генерируем XML
            XDocument xml = GenerateXML(products);

            // Сохраняем XML в файл
            xml.Save("google_merchant.xml");

            Console.WriteLine("XML файл успешно создан: google_merchant.xml");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static async Task<string> DownloadYML(string url)
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(url);
    }

    static List<Product> ParseYML(string ymlContent)
    {
        var deserializer = new DeserializerBuilder().Build();
        var products = deserializer.Deserialize<List<Product>>(ymlContent);
        return products;
    }

    static XDocument GenerateXML(List<Product> products)
    {
        var xDoc = new XDocument(
            new XElement("products",
                new XElement("channel",
                    new XElement("title", "My Store"),
                    new XElement("link", "https://mystore.com"),
                    new XElement("description", "Product feed for Google Merchant"),
                    new XElement("items", products.ConvertAll(p => new XElement("item",
                        new XElement("id", p.Id),
                        new XElement("title", p.Title),
                        new XElement("link", p.Link),
                        new XElement("price", p.Price),
                        new XElement("image_link", p.ImageLink),
                        new XElement("brand", p.Brand),
                        new XElement("availability", p.Availability)
                    )))
                )
            )
        );
        return xDoc;
    }
}

class Product
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Price { get; set; }
    public string ImageLink { get; set; }
    public string Brand { get; set; }
    public string Availability { get; set; }
}
