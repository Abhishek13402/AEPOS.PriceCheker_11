using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace AEPOS.PriceChecker_11.SQLiteClasses.Models
{
    public class ItemInfo
    {

        [Column("sku"), NotNull]
        public int SKU { get; set; }

        [Column("upc"), NotNull]
        public int UPC { get; set; }

        [Column("itemname")]
        public string ITEMNAME { get; set; }


    }
}
