using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.Samples.EntityDataReader;

namespace ConsoleApplication
{
    class Program
    {
        static TimeSpan AddAThousandContacts()
        {
            using (var context = new MyDataStoreEntities())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int i=0; i<1000; i++)
                {
                    var entity = new Contact
                                      {
                                          CustomerId = Guid.NewGuid(),
                                          FirstName = "Ruben",
                                          LastName = "Geers",
                                          EmailAddress = "geersch@gmail.com"
                                      };

                    context.Contacts.AddObject(entity);
                }
                context.SaveChanges();
                stopwatch.Stop();
                return stopwatch.Elapsed;
            }
        }

        static TimeSpan AddAThousandContactsUsingSqlBulkCopy()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var contacts = new List<Contact>();
            for (int i = 0; i < 1000; i++)
            {
                var entity = new Contact
                {
                    CustomerId = Guid.NewGuid(),
                    FirstName = "Ruben",
                    LastName = "Geers",
                    EmailAddress = "geersch@gmail.com"
                };

                contacts.Add(entity);
            }

            var connectionString = 
                ConfigurationManager.ConnectionStrings["MyDataStore"].ConnectionString;
            var bulkCopy = new SqlBulkCopy(connectionString);
            bulkCopy.DestinationTableName = "Contact";
            bulkCopy.WriteToServer(contacts.AsDataReader()); 

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        static void Main()
        {
            // Using Entity Framework & ObjectContext.SaveChanges()
            //var total = new TimeSpan();
            //for (int i = 0; i < 100; i++)
            //{
            //    var timeTaken = AddAThousandContacts();
            //    Console.WriteLine(timeTaken);

            //    total += timeTaken;
            //}
            //Console.WriteLine();
            //var average = new TimeSpan(total.Ticks / 100);
            //Console.WriteLine(String.Format("Average: {0}", average));

            // Using Entity Framework & SqlBulkCopy
            var total = new TimeSpan();
            for (int i = 0; i < 100; i++)
            {
                var timeTaken = AddAThousandContactsUsingSqlBulkCopy();
                Console.WriteLine(timeTaken);

                total += timeTaken;
            }
            Console.WriteLine();
            var average = new TimeSpan(total.Ticks / 100);
            Console.WriteLine(String.Format("Average: {0}", average));

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
