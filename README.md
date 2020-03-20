# Lynda Courses Downloader 
> Download lynda.com courses with the video quality you like.

## Why use this lynda downloader?
* Easy to use
* Cross platform
* Download in the video quality you like
* Doesn't need the lynda desktop app
* Will be adding a GUI soon which will make it the only Lynda Courses Downloader with a GUI because I couldn't find any working ones

## Easy install
Just go to the [releases section](https://github.com/ahmedayman4a/LyndaCoursesDownloader/releases) and download the version that suits your platform.

## Requirements
This program needs geckodriver or chromedriver to be present in the same directory as LyndaCoursesDownloader.exe. The latest version of geckodriver is already present in the release which you van download from [here](https://github.com/ahmedayman4a/LyndaCoursesDownloader/releases) so you don't need to do anything if you have the latest version of Firefox or Firefox version 60 and up.

* If you have Firefox installed , geckodriver needs to be present and it has to be the version compatible with your firefox browser. You can find out [here](https://firefox-source-docs.mozilla.org/testing/geckodriver/Support.html).

* If you have Chrome installed you will need to download the chromedriver which supports your browser version from [here](https://sites.google.com/a/chromium.org/chromedriver/downloads). Old versions of chrome are not recommended.

## How to use
* #### Windows
  Just run the LyndaCoursesDownloader.exe file
* #### Linux
  Open a terminal in the directory of the LyndaCoursesDownloader program then type : 

        chmod 777 ./LyndaCoursesDownloader
   and to run the program type:

        ./LyndaCoursesDownloader

![LyndaCoursesDownloaderDemoGIF](LyndaCoursesDownloader.ConsoleDownloader/img/LyndaCoursesDownloaderTutorial.gif)

## Getting the lynda authentication token cookie
* #### Firefox
  1. Press `Shift+F9` on your keyboard **OR** right click anywhere on the Lynda website , choose "Inspect Element" and click storage.
  2. Look for the word "token" the column "Name". Copy the value and paste it in the LyndaCoursesDownloader program.
* #### Google Chrome
  1. Right click anywhere on the page and click inspect element **OR** press `F12` on your keyboard
  2. Click on the 2 arrows in the top right corner beside the word performance then click Application
  3. Double click on the word "cookies" then click on www.lynda.com
  4. Look for the word "token" the column "Name". Copy the value and paste it in the LyndaCoursesDownloader program.

## Acknowledgments
* Progress bar from [ShellProgressBar Project](https://github.com/Mpdreamz/shellprogressbar) 
