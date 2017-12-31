using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a client.
            ITextAnalyticsAPI client = new TextAnalyticsAPI();
            client.AzureRegion = AzureRegions.Westus;
            client.SubscriptionKey = "<TextAPIKey>";
            var csvDonaldSentiment = new StringBuilder();
            string filePathCsvDonaldSentiment = "C:\\DonaldSentiment.csv";
            string filePathCsvDonaldPhrases = "C:\\DonaldPhrases.csv";
            string filePathCsvDonaldTweets = "C:\\Donald.csv";
            var csvDonaldPhrases = new StringBuilder();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Int16 DocumentId=0;
            Int16 phraseId = 1;
            Int16 BatchId = 1;
            Int16 DocCnt = 1;
            // Getting key-phrases
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            MultiLanguageBatchInput aiBatch;

            MultiLanguageInput languageInput = new MultiLanguageInput();

            List<MultiLanguageInput> listLangues = new List<MultiLanguageInput>();

            using (var reader = new StreamReader(filePathCsvDonaldTweets))
            {
                   while (!reader.EndOfStream && DocCnt < 1000)
                   {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    listLangues.Add(new MultiLanguageInput("en", values[0], RemoveSpecialCharacter(values[1])));
                    DocumentId = Int16.Parse(values[0]);
                    DocCnt++;
                   }


                aiBatch = new MultiLanguageBatchInput(listLangues);

                KeyPhraseBatchResult resultPhrases = client.KeyPhrases(aiBatch);

                // Printing keyphrases
                foreach (var document in resultPhrases.Documents)
                {
                    Console.WriteLine("Document ID: {0} ", document.Id);

                    Console.WriteLine("\t Key phrases:");

                    foreach (string keyphrase in document.KeyPhrases)
                    {
                        Console.WriteLine("\t\t" + keyphrase);
                        var newLine = string.Format("{0},{1},{2}", document.Id, phraseId.ToString(), keyphrase);
                        csvDonaldPhrases.AppendLine(newLine);
                        phraseId++;
                    }
                }

                //after your loop
                File.WriteAllText(filePathCsvDonaldPhrases, csvDonaldPhrases.ToString());

                SentimentBatchResult resultSentiment = client.Sentiment(aiBatch);

                // Printing sentiment results
                foreach (var document in resultSentiment.Documents)
                {
                    Console.WriteLine("Sentiment Score: {1:0.00}", document.Id, document.Score);
                    var newLine = string.Format("{0},{1}", document.Id, document.Score);
                    csvDonaldSentiment.AppendLine(newLine);
                }

                File.WriteAllText(filePathCsvDonaldSentiment, csvDonaldSentiment.ToString());


                
            }

            

        }

        private static string RemoveSpecialCharacter(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == ' ' || c == '%')
                { sb.Append(c); }
            }
            return sb.ToString();
        }
    }
}