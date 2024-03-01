# Academical 2

A work in progress...

## Directory Structure

    AcademicalStep/

The Unity codebase [Document]

    StoryAssemblerRedux/

A reimplmentation of [StoryAssembler.js](https://github.com/LudoNarrative/StoryAssembler/wiki/Authoring-Patterns) in the [Step](https://github.com/ianhorswill/Step) programming language. Ongoing project.

## Deploying web builds to GitHub Pages

Academical uses the [gh-pages npm package](https://www.npmjs.com/package/gh-pages) to deploy webGL builds. You will need a working NPM installation to run the deployment script in `package.json`. You can download NPM with the [latest NodeJs LTS release](https://nodejs.org/en). Once you have that installed run the following two commands to deploy a build.

```bash
# Install gh-pages package
npm install

# Deploy the build to the gh-pages branch
npm run deploy
```

The deployment script requires that a folder at `<path_to_RCRTrainingSim>/AcademicalStep/Build/webgl_academical` contains the `index.html` file and associated build outputs.
