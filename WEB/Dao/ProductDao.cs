using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB.Dao
{
    public class ProductDao
    {
        ShopEntities db = null;
        public ProductDao()
        {
            db = new ShopEntities();
        }

        
        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        }
        
        
    }
}