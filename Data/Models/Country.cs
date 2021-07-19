using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorldCities.Data.Models
{
    [Table("Countries")]
    public class Country
    {
       
        public Country()
        {
        }
    
        ///<summary>
        ///The unique and primary key for Country table
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        ///<summary>
        ///Country Name (in UFT8 format)
        /// </summary>
        public string Name { get; set; }
        ///<summary>
        ///Country Name (in ISO 3166-1 ALPHA-2 format)
        /// </summary>
        [JsonPropertyName("iso2")]
        public string ISO2 { get; set; }
        ///<summary>
        ///Country Name (in ISO 3166-1 ALPHA-3 format)
        /// </summary>
        [JsonPropertyName("iso3")]
        public string ISO3 { get; set; }
        ///<summary>
        /// A list of cities related to this country 
        /// </summary>
        public virtual List<City> Cities { get; set; }



    }
}
