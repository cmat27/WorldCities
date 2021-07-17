using  System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorldCities.Data.Models
{
    [Table("Cities")]
    public class City
    {
       
        public City()
        {
        }
        ///<summary>
        /// unique id and primary key for cities
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        ///<summary>
        /// City Name (in uft8 format
        /// </summary>
        public string Name { get; set; }
        ///<summary>
        /// City name (in ascii format)
        /// </summary>
        public string Name_ASCII { get; set; }
        ///<summary>
        /// City Latitude
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public decimal Lat { get; set; }
        ///<summary>
        /// City Longitud
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public decimal Lon { get; set; }
        ///<summary>
        /// Country ID ForeingKey
        /// </summary>
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        ///<summary>
        /// The Country related to this city 
        /// </summary>
        public virtual Country Country { get; set; }

    }
}
