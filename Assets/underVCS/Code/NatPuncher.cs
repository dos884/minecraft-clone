using Open.Nat;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NatPuncher : MonoBehaviour
{
    private void Start()
    {

		var nat = new NatDiscoverer();
		var cts = new CancellationTokenSource();
		cts.CancelAfter(10000);

		NatDevice device = null;
		IPAddress ip = null;
		var t = nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
		t.ContinueWith(tt =>
		{
			device = tt.Result;
			Debug.Log("device found");
			device.GetExternalIPAsync()
				.ContinueWith(task =>
				{
					int ms = 1000 * 60 * 60;
					ip = task.Result;
					Debug.Log(ip);
					return device.CreatePortMapAsync(new Mapping(Protocol.Udp, 8000, 8000, ms, "ukm"));
				})
				.Unwrap()
				.ContinueWith(task =>
				{
					return 0;
				});

		}, TaskContinuationOptions.OnlyOnRanToCompletion);

	}


    // Update is called once per frame
    void Update()
    {
        
    }
}
