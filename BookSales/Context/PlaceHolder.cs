//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookSales.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class PlaceHolder
    {
        public int idBook { get; set; }
        public int idStorage { get; set; }
        public int stock { get; set; }
    
        public virtual Books Books { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
