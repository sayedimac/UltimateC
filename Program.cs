using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.DataAnnotations;

namespace UltimateC
{

    class Program
    {
        static void Main(string[] args)
        {
            string? DOCINTEL_KEY = Environment.GetEnvironmentVariable("DOCINTEL_KEY");
            string? DOCINTEL_ENDPOINT = Environment.GetEnvironmentVariable("DOCINTEL_ENDPOINT");
            AnalyzeResult result = null;
            if (DOCINTEL_KEY != null && DOCINTEL_ENDPOINT != null)
            {
                // Console.WriteLine($"The value of DOCINTEL_KEY is: {DOCINTEL_KEY}");
                // Console.WriteLine($"The value of DOCINTEL_ENDPOINT is: {DOCINTEL_ENDPOINT}");

                // Call the DOCINTEL API with theAzure  DOCINTEL_KEY and DOCINTEL_ENDPOINT analysing a Contract document with the built-in Contract analysis abilities
                try
                {
                    AzureKeyCredential credential = new AzureKeyCredential(DOCINTEL_KEY);
                    Uri endpoint = new Uri(DOCINTEL_ENDPOINT);
                    DocumentAnalysisClient client = new DocumentAnalysisClient(endpoint, credential);
                    // the Contract is in the sdame directory as the program
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Partnership.pdf");
                    //string contentType = "application/pdf";
                    using FileStream fileStream = File.OpenRead(filePath);
                    AnalyzeDocumentOperation operation = client.AnalyzeDocument(WaitUntil.Completed, "prebuilt-contract", fileStream, null);
                    //operation.WaitForCompletionAsync();
                    result = operation.Value;

                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred: {e.Message}");
                }

                // Display Document Title
                Console.WriteLine($"Document Type:  {result.Documents[0].DocumentType}");

                // Display  ContractId
                if (result.Documents[0].Fields.ContainsKey("ContractId"))
                {
                    Console.WriteLine($"ContractId: {result.Documents[0].Fields["ContractId"].Content}");
                }
                else
                {
                    Console.WriteLine("ContractId: Not Found");
                }

                // Display  ContractDate
                if (result.Documents[0].Fields.ContainsKey("ExecutionDate"))
                {
                    Console.WriteLine($"ExecutionDate: {result.Documents[0].Fields["ExecutionDate"].Content}");
                }
                else
                {
                    Console.WriteLine("ContractDate: Not Found");
                }

                //Display EffectiveDate
                if (result.Documents[0].Fields.ContainsKey("EffectiveDate"))
                {
                    Console.WriteLine($"EffectiveDate: {result.Documents[0].Fields["EffectiveDate"].Content}");
                }
                else
                {
                    Console.WriteLine("EffectiveDate: Not Found");
                }

                //Display ExpirationDate
                if (result.Documents[0].Fields.ContainsKey("ExpirationDate"))
                {
                    Console.WriteLine($"ExpirationDate: {result.Documents[0].Fields["ExpirationDate"].Content}");
                }
                else
                {
                    Console.WriteLine("ExpirationDate: Not Found");
                }

                // Display ContractDuration
                if (result.Documents[0].Fields.ContainsKey("ContractDuration"))
                {
                    Console.WriteLine($"ContractDuration: {result.Documents[0].Fields["ContractDuration"].Content}");
                }
                else
                {
                    Console.WriteLine("ContractDuration: Not Found");
                }

                // Display RenewalDate
                if (result.Documents[0].Fields.ContainsKey("RenewalDate"))
                {
                    Console.WriteLine($"RenewalDate: {result.Documents[0].Fields["RenewalDate"].Content}");
                }
                else
                {
                    Console.WriteLine("RenewalDate: Not Found");
                }

                // Display a list of Parties mentioned in the contract
                Console.WriteLine("Parties:");
                if (result.Documents[0].Fields.TryGetValue("Parties", out DocumentField? PartiesField))
                {
                    if (PartiesField.FieldType == DocumentFieldType.List)
                    {
                        for (int i = 0; i < PartiesField.Value.AsList().Count; i++)
                        {
                            Console.WriteLine($"Party {i + 1} :");

                            if (PartiesField.Value.AsList()[i].FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = PartiesField.Value.AsList()[i].Value.AsDictionary();

                                if (itemFields.TryGetValue("Name", out DocumentField? itemDescriptionField))
                                {
                                    if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                    {
                                        string itemDescription = itemDescriptionField.Value.AsString();

                                        Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Jurisdictions:");
                if (result.Documents[0].Fields.TryGetValue("Jurisdictions", out DocumentField? JurisdictionsField))
                {
                    if (JurisdictionsField.FieldType == DocumentFieldType.List)
                    {

                        for (int i = 0; i < JurisdictionsField.Value.AsList().Count; i++)
                        {
                            Console.WriteLine($"Jurisdiction {i + 1} :");

                            if (JurisdictionsField.Value.AsList()[i].FieldType == DocumentFieldType.Dictionary)
                            {
                                IReadOnlyDictionary<string, DocumentField> itemFields = JurisdictionsField.Value.AsList()[i].Value.AsDictionary();

                                if (itemFields.TryGetValue("Region", out DocumentField? itemDescriptionField))
                                {
                                    if (itemDescriptionField.FieldType == DocumentFieldType.String)
                                    {
                                        string itemDescription = itemDescriptionField.Value.AsString();

                                        Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("Some environment Variables not found... you  need a DOCINTEL_KEY and a DOCINTEL_ENDPOINT env variable");
            }
        }
    }
}