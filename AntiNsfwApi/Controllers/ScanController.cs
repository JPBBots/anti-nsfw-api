using Microsoft.AspNetCore.Mvc;
using NsfwSpyNS;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AntiNsfwApi.Controllers
{
    public class Prediction
    {
        public float percentage
        {
            get;
            set;
        }

        public bool nsfw
        {
            get;
            set;
        }

        public object extended
        {
            get;
            set;
        }
    }

    public class PostToScan
    {
        public System.Uri url
        {
            get;
            set;
        }
    }

    [ApiController]
    [Route("/scan")]
    public class ScanController : ControllerBase
    {
        private readonly NsfwSpy spy = new();

        private Prediction makePrediction (NsfwSpyResult res, bool debug = false)
        {
            Prediction prediction = new()
            {
                nsfw = false,
                percentage = 0,
                extended = null
            };

            float percentage = 0;

            float[] percentages = { res.Pornography, res.Hentai };

            foreach (float percent in percentages)
            {
                if (percent > percentage) percentage = percent;
            }

            if (percentage > 0.50 && res.IsNsfw)
            {
                prediction.nsfw = true;
                prediction.percentage = percentage;
            }

            if (debug)
            {
                prediction.extended = res;
            }

            return prediction;
        }

        [HttpPost]
        [Route("/scan/image")]
        public async Task<Prediction> ScanImage([FromBody]PostToScan info, [FromQuery(Name = "debug")] bool debug)
        {
            var res = await spy.ClassifyImageAsync(info.url);

            return makePrediction(res, debug);
        }

/*        [HttpPost]
        [Route("/scan/video")]
        public async Task<Prediction> ScanVideo([FromBody] PostToScan info, [FromQuery(Name = "debug")] bool debug)
        {
            Prediction p = new();

            var res = await spy.ClassifyVideoAsync(info.url);

            p.nsfw = res.IsNsfw;
            p.percentage = 1;
            if (debug)
            {
                p.extended = res;
            }

            return p;
        }*/
    }
}
