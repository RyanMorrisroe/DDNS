# CloudFlare DDNS

A super basic DDNS updater that runs on Windows for CloudFlare. I just run it as a scheduled task that calls the exe, but you could pretty easily change it to work as a service. To make this work if you build it locally, recommend adding an appsettings.json file alongside the exe in the following format:

{
  "cloudFlareToken": "{YOUR TOKEN VALUE HERE}"
}

Your token should have read and edit permissions on all DNS entries in you the zone(s) you want to update.
