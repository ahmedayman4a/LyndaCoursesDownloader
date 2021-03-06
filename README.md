# Lynda Courses Downloader 
> Download lynda.com courses with the video quality you like.

## Why use this lynda downloader?
* The only Lynda Courses Downloader with a GUI as I couldn't find any other working ones
* Easy to use
* Cross platform
* Download in the video quality you like
* Doesn't need the lynda desktop app
* Download Exercise files automatically
* Download multiple courses at a time
* Automatically extract lynda.com token from Chrome, Firefox or Microsoft Edge

## Easy install
Just go to the [releases section](https://github.com/ahmedayman4a/LyndaCoursesDownloader/releases), download the version that suits your platform and make sure you follow the requirements.

## Requirements
At least .Net Framework 4.6.2 which comes pre-installed with Windows 10 Anniversary Update (Version 1607)

## How to use
* #### Windows
  Just run the Setup file. A shortcut will be added to your desktop and start menu
* #### Linux
  Open a terminal in the directory of the LyndaCoursesDownloader program then type : 

        chmod 777 ./LyndaCoursesDownloader
   and to run the program type:

        ./LyndaCoursesDownloader

![LyndaCoursesDownloaderDemoGIF](LyndaCoursesDownloader.ConsoleDownloader/img/LyndaDownloaderDemo.gif)

## Getting the lynda authentication token cookie
* #### You can now extract the token from your browser's default profile if you are logged into lynda.com by pressing `Extract Token`. If it didn't work for you, manually get the token as follows:
* **Firefox**
  1. Press `Shift+F9` on your keyboard **OR** right click anywhere on the Lynda website , choose "Inspect Element" and click storage.
  2. Look for the word "token" the column "Name". Copy the value and paste it in the LyndaCoursesDownloader program.
  ![LyndaCoursesDownloader firefox token tutorial gif](LyndaCoursesDownloader.ConsoleDownloader/img/LyndaTokenTutorialFirefox.gif)
* **Google Chrome**
  1. Right click anywhere on the page and click inspect element **OR** press `F12` on your keyboard
  2. Click on the 2 arrows in the top right corner beside the word performance then click Application
  3. Double click on the word "cookies" then click on www.lynda.com
  4. Look for the word "token" the column "Name". Copy the value and paste it in the LyndaCoursesDownloader program.
  ![LyndaCoursesDownloader chrome token tutorial gif](LyndaCoursesDownloader.ConsoleDownloader/img/LyndaTokenTutorialChromeCompressed.gif)
## Any Questions? Issues? Recommendations?
Just create an [issue](https://github.com/ahmedayman4a/LyndaCoursesDownloader/issues/new/choose) and I will reply as soon as I can.
## Acknowledgments
* Progress bar from [ShellProgressBar Project](https://github.com/Mpdreamz/shellprogressbar) 
* Extractor uses [Curl](https://curl.se/)
* Installer and Updater from [Squirrel](https://github.com/Squirrel/Squirrel.Windows)
