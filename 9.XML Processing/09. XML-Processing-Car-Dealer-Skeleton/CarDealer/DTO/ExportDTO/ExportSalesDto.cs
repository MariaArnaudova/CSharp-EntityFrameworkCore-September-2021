﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
    [XmlType("sale")]
  public  class ExportSalesDto
    {   
        [XmlElement("car")]
        public CarSaleOutputMpdel CarSale { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]

        public  string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }


        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }

    }
}
