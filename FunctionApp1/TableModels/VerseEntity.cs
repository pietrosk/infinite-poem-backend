using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace FunctionApp1.TableModels
{
    public class VerseEntity : TableEntity
    {
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
