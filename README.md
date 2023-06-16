# CloudFlare DDNS

A super basic DDNS updater that runs on Windows for CloudFlare. To make this work if you build it locally, recommend adding an appsettings.json file alongside the exe in the following format:

{
  "cloudFlareToken": "{YOUR TOKEN VALUE HERE}"
}

Your token should have read and edit permissions on all DNS entries in you the zone(s) you want to update.
