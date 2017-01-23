using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoConstruct.AmazingRace.Models;
using CoConstruct.AmazingRace.Services;
using FreshMvvm;
using PropertyChanged;
using Xamarin.Forms;

namespace CoConstruct.AmazingRace.PageModels
{
    [ImplementPropertyChanged]
    public class LegsPageModel : FreshBasePageModel
    {
        public readonly IDirectionsService _directionsService;
        public LegsPageModel(IDirectionsService directionsService)
        {
            _directionsService = directionsService;
        }

        public List<PitStop> PitStops { get; set; }
        public List<Leg> Legs { get; set; }

        private Leg _selectedLeg;
        public Leg SelectedLeg
        {
            get { return _selectedLeg; }
            set
            {
                _selectedLeg = value;
                if (_selectedLeg == null) return;
                LegSelectedCommand.Execute(_selectedLeg);
                _selectedLeg = null;
            }
        }

        public Command<Leg> LegSelectedCommand
        {
            get
            {
                return new Command<Leg>(async (leg) =>
                {
                    await CoreMethods.PushPageModel<DirectionsPageModel>(leg);
                });
            }
        }


        public override void Init(object initData)
        {
            // list should already be ordered
            PitStops = (List<PitStop>) initData;

            // create the legs
            Legs = PitStops.Zip(PitStops.Skip(1), (start, stop) => new Leg(start, stop)).ToList();
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            foreach (var leg in Legs)
            {
                if (leg.Steps == null)
                    leg.Steps = await _directionsService.GetDirections(leg.From.Address, leg.To.Address);
            }
            //var x = await _directionsService.GetDirections("1783 Bledsoe Dr., Bellbrook OH",
            //    "1600 Pennsylvania Ave., Washington DC");
            base.ViewIsAppearing(sender, e);
        }
    }
}
