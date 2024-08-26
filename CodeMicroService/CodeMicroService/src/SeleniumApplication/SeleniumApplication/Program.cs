using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumApplication.Consumers;
using SeleniumApplication.Data;
using SeleniumApplication.Senders;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<JobSearchConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        cfg.ReceiveEndpoint("jobsearch-queue", e =>
                        {
                            e.ConfigureConsumer<JobSearchConsumer>(context);
                        });
                    });
                });

                services.AddTransient<JobSearchSender>();
            })
            .Build();

        await host.RunAsync();

        Console.ReadKey();
    }




    private static void FindJobs(string keyword, string url, SeleniumApplicationDbContext context)
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        using IWebDriver driver = new ChromeDriver();
        var jobs = new List<SeleniumApplication.Models.Job>();

        try
        {
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var searchInput = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("search__input")));
            searchInput.SendKeys(keyword);

            Thread.Sleep(2000);


            int previousItemCount = 0;
            int currentItemCount = 0;
            int maxAttempts = 5; // En fazla deneme sayısı
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                // 'list__scroller' sınıfına sahip div elementini kaydırıyoruz
                ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('scroller_desctop').scrollTop = document.getElementById('scroller_desctop').scrollHeight;");
                Thread.Sleep(3000); // Bekleme süresi, içeriklerin yüklenmesi için gerekli zaman tanır

                // Şu anki içerik sayısını kontrol ediyoruz
                previousItemCount = currentItemCount;
                currentItemCount = driver.FindElements(By.ClassName("list__item")).Count;

                // Eğer içerik sayısı artmıyorsa, döngüyü durdur
                if (currentItemCount == previousItemCount)
                {
                    attempts++; // Deneme sayısını artırıyoruz
                    if (attempts >= maxAttempts)
                    {
                        break; // Maksimum deneme sayısına ulaşıldığında döngüden çık
                    }
                }
                else
                {
                    attempts = 0; // Yeni içerik yüklendiğinde deneme sayısını sıfırla
                }
            }

            var listItems = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("list__item")));

            Console.WriteLine(listItems.Count);
            foreach (var item in listItems)
            {
                if (item.GetAttribute("class").Contains("list__item--reklam"))
                    continue;

                var anchor = item.FindElement(By.ClassName("list__item__text"));
                if (anchor == null)
                    continue;

                var h3 = item.FindElement(By.ClassName("list__item__title"));

                var jobTitle = h3?.Text ?? string.Empty;
                var companyName = anchor.Text.Replace(jobTitle, "").Trim();

                var job = new SeleniumApplication.Models.Job
                {
                    Keyword = keyword,
                    Title = jobTitle,
                    Url = anchor.GetAttribute("href"),
                    CompanyName = companyName
                };

                jobs.Add(job);
            }

            context.Jobs.AddRange(jobs);
            context.SaveChanges();
        }
        catch (WebDriverException ex)
        {
            Console.WriteLine("An error occurred while navigating to the URL: " + ex.Message);
        }

        driver.Quit();
    }
}

