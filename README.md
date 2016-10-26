# WebCamImageCollector
Simple periodic webcam image collector for IoT and UWP. It consists of two projects:

**Background** is a IoT headless application with background task. It implements simple web server throught wich capturing images in defined interval can be started or stopped. This web server is secured using authentication token. This project is also included in the RemoteControl.

**RemoteControl** is a UWP application that can run both on dekstop and mobile Windows 10. You can define list of servers and control them over HTTP. Each server you start, stop and download latest image from it. Beside controlling remote server, you can configure a local instance of Background project.

## Licence

[Apache 2.0](blob/master/LICENSE)
