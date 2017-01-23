using CoConstruct.AmazingRace.Models;
using FreshMvvm;
using Xamarin.Forms;

namespace CoConstruct.AmazingRace.PageModels
{
    public class DirectionsPageModel : FreshBasePageModel
    {
        public DirectionsPageModel()
        {
            
        }

        public string Html { get; set; }
        public Leg Leg { get; set; }

        public override void Init(object initData)
        {
            Leg = initData as Leg;
            if (Leg == null) return;
            var s = $"Origin:<br/>{Leg.From.Address} <br/>Destination:<br/> {Leg.To.Address}<br/><br/>";
            Html = s + Leg.StepsText;
        }
    }
}