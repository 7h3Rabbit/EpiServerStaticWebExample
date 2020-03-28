# EpiServer - StaticWeb (Example)

Project is built on top of vanilla EpiServer Alloy website.

## Introduction ##

Project showcasing creating a static website but still using EpiServer CMS for editorial changes.

**Pro**

- Reliable serverside response time
- Very easy to scale up
- No database dependency for visitor
- No serverside code requried
- Very secure (hard to hack static pages)

**Con**

- No serverside dynamic content can be used
- No serverside personalized content can be used
- Only pages inheriting from PageData will trigger page write
- Only block inheriting from BlockBata will trigger page write

## Requirement ##

- EpiServer 7.5+
- .Net 4.7+
- All pages need to inherit from PageData
- All blocks needs to inherit from BlockData


## Installation ##

- Copy StaticWebEventInit.cs into your project

 
