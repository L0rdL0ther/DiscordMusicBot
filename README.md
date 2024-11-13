# MozartCakma - Discord Music Bot

`MozartCakma` is a music bot for Discord that allows you to play music in your voice channels. The bot supports streaming music from YouTube links and can be controlled using specific commands.

## Features

- **FFmpeg & Opus support**: Powerful libraries for audio streaming.
- **yt-dlp integration**: Play music from YouTube.
- **.NET 8.0 SDK**: Utilizes the latest technology for modern development and performance.
- **Track system**: The bot tracks the current song. The track is cleared when the bot is stopped.

## Important Notes

- This bot has **only been tested on Linux-based systems**. It has **not been tested on Windows**, so there is a risk that it might not work on Windows.
- The bot currently supports only **YouTube links**. Other platforms are **not supported**.
- This is **not a stable version**, and there may be bugs or errors.
- You will need to add a **Bot Token** and **Prefix** in the settings for the bot to work.
## ⚠️ ⚠️ ⚠️ A **YouTube API key** is required to search for videos. You can get it by signing up for an account at [RapidAPI YouTube Search and Download API](https://rapidapi.com/h0p3rwe/api/youtube-search-and-download) and adding the key in the settings.


## Features

- **FFmpeg & Opus support**: Powerful libraries for audio streaming.
- **yt-dlp integration**: Play music from YouTube and other video platforms.
- **.NET 8.0 SDK**: Utilizes the latest technology for modern development and performance.

## Installation

### Ubuntu

Follow these steps to set up and run MozartCakma on Ubuntu:

#### 1. Install Required Dependencies
Start by installing the necessary dependencies:

```bash
sudo apt-get update
sudo apt-get install libopus0 libopus-dev snap
sudo snap install --edge yt-dlp
sudo apt install ffmpeg
```

#### 2. Install .NET SDK

`MozartCakma` requires .NET SDK 8.0. You can follow the official guide to install it on Ubuntu:

[Install .NET SDK 8.0 on Ubuntu](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?tabs=dotnet9&pivots=os-linux-ubuntu-2410)

```bash
sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-8.0
```

#### 3. Run the Project

To run the bot:

[Download the app files here.](https://github.com/L0rdL0ther/DiscordMusicBot/releases/tag/Download)

Navigate to the project directory and run the .NET application:

```bash
./MozartCakma
```

---

### Windows

Follow these steps to set up and run `MozartCakma` on Windows:

#### 1. Install Required Dependencies

To install FFmpeg and yt-dlp on Windows, you can use the following methods:

##### Install FFmpeg:

```bash
winget install ffmpeg
```

##### Install yt-dlp:

Download and install yt-dlp from the [Official GitHub page](https://github.com/yt-dlp/yt-dlp?tab=readme-ov-file#installation).

After downloading, make sure to place the `yt-dlp` file in the same directory as your bot's executable.

#### 2. Install .NET SDK

Follow the official guide to install .NET SDK 8.0 on Windows:

[Install .NET SDK 8.0 on Windows](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

#### 3. Run the Project

To run the bot on Windows:

[Download the app files here.](https://github.com/L0rdL0ther/DiscordMusicBot/releases/tag/Download)

Navigate to the project directory and run the .NET application:

- **Double-click** `MozartCakma` app.

Or, from PowerShell (recommended):

```bash
./MozartCakma
```

**Note**: This has not been fully tested on Windows, so there may be issues. If you encounter any problems, please report them on the GitHub Issues page.

---

### Arch Linux

Follow these steps to set up and run MozartCakma on Arch Linux:

#### 1. Install Required Dependencies
Start by installing the necessary dependencies on Arch Linux:

```bash
sudo pacman -Syu --noconfirm && pacman -S --noconfirm \
    ffmpeg \
    opus \
    libsodium \
    yt-dlp \
    glibc \
    gcc-libs
```

#### 2. Install .NET SDK

`MozartCakma` requires .NET SDK 8.0. You can install it on Arch Linux with the following commands:

```bash
sudo pacman -S --noconfirm \
    dotnet-sdk-8.0 \
    dotnet-runtime-8.0 \
    mono
```

#### 3. Run the Project

To run the bot:

[Download the app files here.](https://github.com/L0rdL0ther/DiscordMusicBot/releases/tag/Download)

Navigate to the project directory and run the .NET application:

```bash
./MozartCakma
```

---

### Docker (Arch Linux)

You can also run MozartCakma using Docker. The **Dockerfile** for Arch Linux is available. You can access it via the link below:

Arch Linux Dockerfile

Make sure Docker is installed on your system, and follow these steps:

```bash
# Start with a base image that is based on Arch Linux
FROM archlinux:latest AS runtime

# Update the system and install necessary dependencies
RUN pacman -Syu --noconfirm && pacman -S --noconfirm \
    ffmpeg \
    opus \
    libsodium \
    yt-dlp \
    glibc \
    gcc-libs \
    && pacman -Scc --noconfirm  # Clean up unnecessary files

# Install .NET 8.0 SDK and Runtime on Arch Linux
RUN pacman -S --noconfirm \
    dotnet-sdk-8.0 \
    dotnet-runtime-8.0 \
    mono

# Set the working directory
WORKDIR /app

# Copy all project files from the local directory into the Docker container
COPY ./ /app/

# Start the "MozartCakma" executable
ENTRYPOINT ["./MozartCakma"]

```

Ensure that your Docker container has the necessary environment variables for the bot to function (Bot Token, Prefix, YouTube API Key).


---

## Usage

Once the bot is running, you can use the following commands:

- `!search <query>`: Search for a song on YouTube.
- `!play <YouTube URL>`: Play music from a YouTube link.
- `!stop`: Stop the current song and clear the track.

### Bot Settings

To use the bot, you must configure the following settings:

1. **Bot Token**: You must enter your Discord bot token in the configuration file.
2. **Prefix**: You must define a command prefix (e.g., `!`).
3. **YouTube API Key**: You need to create an API key from the [RapidAPI YouTube Search and Download API](https://rapidapi.com/h0p3rwe/api/youtube-search-and-download) and add it to the configuration.

### Configuration File

After running the bot once, a `config.json` file will be created. The configuration file will look like this:

```json
{
  "BotToken": "YOUR_TOKEN_HERE",
  "Prefix": "!",
  "RapidKey": "YOUR_API_KEY_HERE"
}
```

Make sure to replace `YOUR_TOKEN_HERE` and `YOUR_API_KEY_HERE` with your actual Discord bot token and YouTube API key.

---

## Contributing

If you'd like to contribute to the `MozartCakma` project, please follow these steps:

1. Fork this repository.
2. Create a new branch.
3. Make your changes.
4. Commit your changes.
5. Open a pull request.

---

## License

This project is licensed under the [MIT License](https://github.com/L0rdL0ther/DiscordMusicBot/blob/main/LICENSE).

---

## Contact

If you have any questions or feedback, feel free to open an issue on the [GitHub Issues page](https://github.com/L0rdL0ther/DiscordMusicBot/issues).
