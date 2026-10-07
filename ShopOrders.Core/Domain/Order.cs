using System;
using System.Collections.Generic;

namespace ShopOrders.Core.Domain
{
    public class Order
    {
        private readonly List<string[]> _lines = new List<string[]>();

        public string Id { get; private set; }
        public string CustomerName { get; private set; }
        public int Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Order(string id, string customerName)
        {
            Id = id;
            CustomerName = customerName;
            CreatedAt = DateTime.Now;
        }

        public void AddLine(string sku, int quantity, decimal unitPrice)
        {
            string[] lineData = new string[3];
            lineData[0] = sku; lineData[1] = quantity.ToString(); lineData[2] = unitPrice.ToString();
            _lines.Add(lineData);
        }

        public decimal CalculateTotal(bool isRegularCustomer)
        {
            decimal total = 0; int lineCount = 0;
            for (int i = 0; i < _lines.Count; i++)
            {
                int k = int.Parse(_lines[i][1]);
                decimal c = decimal.Parse(_lines[i][2]);
                total = total + k * c;
                lineCount = lineCount + 1;
            }
            if (isRegularCustomer == true && total > 1000) { total = total * 0.9m; } else if (total > 5000) { total = total * 0.85m; } else { total = total; }
            if (lineCount > 10) total = total - 100;
            if (total < 0) total = 0;
            total = total + total * 0.2m;
            return Math.Round(total, 2);
        }

        public bool TryChangeStatus(int newStatus)
        {
            if (Status == 0 && newStatus == 1) { Status = 1; return true; }
            if (Status == 1 && newStatus == 2) { Status = 2; return true; }
            if (Status == 0 && newStatus == 3) { Status = 3; return true; }
            return false;
        }

        public bool IsValid()
        {
            if (Id != null && Id != "" && CustomerName != null && CustomerName.Length > 2 && _lines.Count > 0 && _lines.Count < 100 && Status >= 0 && Status <= 3)
                return true;
            else
                return false;
        }

        public string BuildReport()
        {
            string s = "";
            for (int i = 0; i < _lines.Count; i++)
            {
                s = s + "Товар: " + _lines[i][0] + "; кількість: " + _lines[i][1] + "; ціна: " + _lines[i][2] + "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
            }
            s = s + "Разом: " + CalculateTotal(false) + "\n";
            return s;
        }

        public static Order FindById(List<Order> orders, string orderId)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Id == orderId) { return orders[i]; }
            }
            return null;
        }
    }
}
