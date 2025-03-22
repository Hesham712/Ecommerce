﻿namespace Ecommerce.Models
{
    public class Cart : AbstractModel
    {

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<CartProduct> CartProducts { get; set; }

    }
}
