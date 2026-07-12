# Étape 1 : Image de base runtime .NET 7
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# --- Installer Chrome & ChromeDriver dans l'image finale ---
# Chrome version stable connue et publiée
ENV CHROME_VERSION=129.0.6668.100

RUN apt-get update && apt-get install -y \
    wget unzip fonts-liberation libappindicator3-1 libnss3 libxss1 \
    libasound2 libatk-bridge2.0-0 libgtk-3-0 xdg-utils ca-certificates \
 && wget -q "https://storage.googleapis.com/chrome-for-testing-public/${CHROME_VERSION}/linux64/chrome-linux64.zip" \
 && unzip -q chrome-linux64.zip -d /opt/chrome \
 && ln -s /opt/chrome/chrome-linux64/chrome /usr/bin/google-chrome \
 && wget -q "https://storage.googleapis.com/chrome-for-testing-public/${CHROME_VERSION}/linux64/chromedriver-linux64.zip" \
 && unzip -q chromedriver-linux64.zip -d /tmp \
 && mv /tmp/chromedriver-linux64/chromedriver /usr/local/bin/chromedriver \
 && chmod +x /usr/local/bin/chromedriver \
 && rm -rf /var/lib/apt/lists/* chrome-linux64.zip chromedriver-linux64.zip /tmp/chromedriver-linux64

# 1. Mise à jour des paquets
# 2. Installation de Tesseract OCR et des langues Français/Anglais
# 3. Installation de Fontconfig et de polices libres pour ImageSharp (filigrane)
# 4. Nettoyage du cache apt pour garder une image Docker légère
RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    tesseract-ocr-fra \
    tesseract-ocr-eng \
    fontconfig \
    fonts-liberation \
    && rm -rf /var/lib/apt/lists/*

# (Optionnel) Rafraîchir le cache des polices pour s'assurer qu'ImageSharp les voit bien
RUN fc-cache -f -v


# Étape 2 : Image de build .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copier les fichiers de projet (pour restaurer les dépendances plus vite)
COPY ["front/TesterLab.csproj", "front/"]
COPY ["backend/TesterLab.Application/TesterLab.Applications.csproj", "backend/TesterLab.Application/"]
COPY ["backend/TesterLab.Domain/TesterLab.Domain.csproj", "backend/TesterLab.Domain/"]
COPY ["backend/TesterLab.Infrastructure.Selenium/TesterLab.Infrastructure.Selenium.csproj", "backend/TesterLab.Infrastructure.Selenium/"]
COPY ["backend/TesterLab.Infrastructure/TesterLab.Infrastructure.csproj", "backend/TesterLab.Infrastructure/"]

# Restaurer les dépendances
RUN dotnet restore "front/TesterLab.csproj"

# Copier tout le reste
COPY . .

# Construire l'application
WORKDIR "/src/front"
RUN dotnet build "TesterLab.csproj" -c Release -o /app/build

# Étape 3 : Publication
FROM build AS publish
RUN dotnet publish "TesterLab.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Étape 4 : Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Chrome + ChromeDriver sont déjà installés depuis la première étape
ENV PATH="${PATH}:/usr/local/bin"

# Démarre l'application
ENTRYPOINT ["dotnet", "TesterLab.dll"]
