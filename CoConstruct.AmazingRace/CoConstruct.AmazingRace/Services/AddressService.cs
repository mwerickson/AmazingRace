using System.Linq;
using System.Threading.Tasks;
using CoConstruct.AmazingRace.Extensions;
using Xamarin.Forms.GoogleMaps;

namespace CoConstruct.AmazingRace.Services
{
    public interface IAddressService
    {
        Task<string> GetNormalizedAddress(string rawAddress);
        Task<Position> GetPosition(string rawAddress);
        Task<string> GetAddress(Position pos);
    }

    public class AddressService : IAddressService
    {
        private Geocoder _geoCoder;

        public AddressService()
        {
            _geoCoder = new Geocoder();
        }

        public virtual async Task<string> GetNormalizedAddress(string rawAddress)
        {
            var pos = await GetPosition(rawAddress);
            if (pos.Equals(default(Position))) return null;

            return await GetAddress(pos);
        }

        public async Task<Position> GetPosition(string rawAddress)
        {
            var positions = await _geoCoder.GetPositionsForAddressAsync(rawAddress);
            return positions == null ? default(Position) : positions.FirstOrDefault();
        }

        public async Task<string> GetAddress(Position pos)
        {
            // geocode the position to get the normalized address
            var addresses = await _geoCoder.GetAddressesForPositionAsync(pos);
            if (addresses == null) return null;

            var address = addresses.FirstOrDefault();

            // clean up the address, there is a known issue having junk (non-printable) characters in the address.
            address = address?.Replace("United States", "");
            address = address?.Replace("\n", " ");
            address = address?.RemoveAsciiChar((char)226);
            address = address?.RemoveAsciiChar((char)128);
            address = address?.RemoveAsciiChar((char)142);
            return address;
        }
    }
}