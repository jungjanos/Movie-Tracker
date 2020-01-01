Ch9.Services.YtExplodeVideoService uses https://github.com/Tyrrrz/YoutubeExplode to parse direct mediastream Url-s from the Youtube webpage.
Because of this, it is considered dirty as stream riping is not acceptable by GooglePlay.

I have implemented the component as a separate project so the Ch9.Services assembly itself does not have to take a dependency to YoutubeExplode.
This makes it easier to exclude from build/publishing process when targeting GooglePlay.

Ch9.Services.YtExplodeVideoService also builds on the VideoService component in Ch9.Services to query and display Youtube video metadata information



