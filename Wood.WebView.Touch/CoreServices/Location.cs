using System;
using System.Collections.Generic;
using Wood.Core;
using System.Timers;
using CoreLocation;
using UIKit;

namespace Wood.CoreService
{
    class Location:ServiceBase
    {
		static readonly Dictionary<string, LocationManager> Mgrs = new Dictionary<string, LocationManager>();
        public override string ServiceName
        {
            get { return "Location"; }
        }

        public Location()
        {
            //定位，这里只是模拟
            
			AddMethod("currentPos", (core,args) =>
            {
					var Mgr=new LocationManager(core,args);
					EventHandler<LocationUpdatedEventArgs> handler;
					handler=(sender,e)=>{
						var c = (sender as LocationManager).WoodCore;
						var a=(sender as LocationManager).ServiceArgs;
						var location = e.Location;
						c.InvokeCallback(a.CallbackName, new { lng =location.Coordinate.Longitude, lat = location.Coordinate.Latitude });
						(sender as LocationManager).StopUpdatingLocation();
					};
					Mgr.LocationUpdated+=handler;
					UIApplication.Notifications.ObserveDidEnterBackground((s,a)=>{
						Mgr.LocationUpdated-=handler;
					});
					UIApplication.Notifications.ObserveDidBecomeActive((s,e)=>{
						Mgr.LocationUpdated+=handler;
					});
					Mgr.StartUpdatingLocation();
    		});
            AddMethod("watch", (core,args) =>
            {
					var Mgr=new LocationManager(core,args);

					Mgr.LocationUpdated+=LocationUpdated;
					UIApplication.Notifications.ObserveDidEnterBackground((s,a)=>{
						Mgr.LocationUpdated-=LocationUpdated;
					});
					UIApplication.Notifications.ObserveDidBecomeActive((s,e)=>{
						Mgr.LocationUpdated+=LocationUpdated;
					});
					Mgr.StartUpdatingLocation();
					return Mgr.Id.ToString("N");
            });
            AddMethod("unwatch", (core,args) =>
            {
                var id = args.GetPn(0,(string)null);
					if (!string.IsNullOrWhiteSpace(id) && Mgrs.ContainsKey(id))
                {
						var mgr = Mgrs[id];
						Mgrs.Remove(id);
						if (mgr != null)
                    {
							mgr.StopUpdatingLocation();
							mgr = null;
                    }
                }
            });
        }
		void LocationUpdated(object sender,LocationUpdatedEventArgs e){
			var core = (sender as LocationManager).WoodCore;
			var args=(sender as LocationManager).ServiceArgs;
			var location = e.Location;
			core.InvokeCallback(args.CallbackName, new { lng =location.Coordinate.Longitude, lat = location.Coordinate.Latitude });
		}
		class LocationUpdatedEventArgs:EventArgs{
			public CLLocation Location{private set;get;}
			public LocationUpdatedEventArgs(CLLocation location){
				Location=location;
			}
		}
		class LocationManager{
			public event EventHandler<LocationUpdatedEventArgs>  LocationUpdated=delegate {};
			CLLocationManager mgr;
			WoodCore core;
			public WoodCore WoodCore{get{ return core;}}
			ServiceArgs args;
			public ServiceArgs ServiceArgs{get{ return args;}}
			public Guid Id{ get; }
			public LocationManager(WoodCore core,ServiceArgs args){
				this.core=core;
				this.args=args;
				Id=Guid.NewGuid();
				mgr=new CLLocationManager();
				if(UIDevice.CurrentDevice.CheckSystemVersion(8,0)){
					mgr.RequestAlwaysAuthorization();
				}
				if(CLLocationManager.LocationServicesEnabled){
					mgr.DesiredAccuracy=1;
					//StartUpdatingLocation();
					mgr.LocationsUpdated+=delegate(object sender,CLLocationsUpdatedEventArgs e) {
						LocationUpdated(this,new LocationUpdatedEventArgs(e.Locations[e.Locations.Length-1]));	
					};
				}
			}
			public void StartUpdatingLocation(){
				
				mgr.StartUpdatingLocation ();
			}
			public void StopUpdatingLocation(){
				mgr.StopUpdatingLocation ();
			}

		}
    }
}