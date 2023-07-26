FROM mcr.microsoft.com/dotnet/sdk:7.0

# [Optional] Uncomment this section to install additional OS packages.
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    curl \
    wget \
    ca-certificates \
    libicu-dev \
    && rm -rf /var/lib/apt/lists/*

# Install NodeJs v18.x
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | /bin/bash - \
    && apt-get install -y nodejs

# Register Neos Nuget source
RUN dotnet nuget add source "https://pkgs.dev.azure.com/POLE-ERP-GROUPEISA/Neos-Public/_packaging/Neos-Public/nuget/v3/index.json" -n "Neos-Public"

# Create dev certificate
RUN dotnet dev-certs https --trust

# Install Neos
RUN dotnet tool install GroupeIsa.Neos.Console --global --ignore-failed-sources --no-cache --interactive --verbosity minimal --version $version

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1


ENV PATH="${PATH}:/root/.dotnet/tools"
ENV NODE_OPTIONS="--max_old_space_size=16384"

ENTRYPOINT "/bin/bash"