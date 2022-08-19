**This repository is no longer maintained, please move to [Kit086/kit.blog](https://github.com/Kit086/kit.blog)**

[![Deploy blog](https://github.com/tatwd/blog/actions/workflows/ci.yml/badge.svg)](https://github.com/tatwd/blog/actions/workflows/ci.yml)

Dev (.net 6.0)

```sh
# Generate files to dist/
# or ./build.sh
dotnet run --project src/blog.csproj \
    --cwd ./ \
    --posts ./posts \
    --theme ./theme \
    --dist ./dist

# Preview by dotnet-serve or others
dotnet serve --directory dist
```
