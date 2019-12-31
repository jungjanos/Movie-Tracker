Ch9.Services.YtExplodeVideoService uses https://github.com/Tyrrrz/YoutubeExplode to parse direct mediastream Url-s from the Youtube webpage.
For this it is considered dirty stream riping is not acceptable by GooglePlay.

It is implemented as a separate project so the Ch9.Services assembly itself does not have to take a dependency to YoutubeExplode.
This makes it easier to exclude from build/publishing process when targeting GooglePlay.

Ch9.Services.YtExplodeVideoService also builds on the VideoService component in Ch9.Services to query and display Youtube video metadata information



