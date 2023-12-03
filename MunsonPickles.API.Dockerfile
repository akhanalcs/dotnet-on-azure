# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source

# copy csproj files of app and libraries, and restore as distinct layers
COPY "MunsonPickles.API/*.csproj" "MunsonPickles.API/"
COPY "MunsonPickles.Shared/*.csproj" "MunsonPickles.Shared/"
RUN dotnet restore "MunsonPickles.API/MunsonPickles.API.csproj"

# copy and build app and libraries
COPY "MunsonPickles.API/" "MunsonPickles.API/"
COPY "MunsonPickles.Shared/" "MunsonPickles.Shared/"
WORKDIR "/source/MunsonPickles.API"
# Currently it doesn't work with --no-restore flag so I've removed it. I plan to use it in the future.
# I opened this issue in Github for this: https://github.com/dotnet/sdk/issues/37291
RUN dotnet publish -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-jammy-chiseled-composite
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./MunsonPickles.API"]