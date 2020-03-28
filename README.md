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

**Con/limitations**

- No serverside dynamic content can be used
- No serverside personalized content can be used
- Only pages inheriting from PageData will trigger page write
- Only block inheriting from BlockBata will trigger page write
- Only supports following types:
  - css (only support dependencies declared in url())
  - javascript
  - Web fonts (woff and woff2)
  - Images (png, jpg, jpeg, jpe, gif, webp)
  - documents (pdf)
  - Icons (ico)

## Requirements ##

- EpiServer 7.5+
- .Net 4.7+
- All pages need to inherit from PageData
- All blocks needs to inherit from BlockData
- Website has to return pages, javascript and css as UTF-8


## Installation ##

- Copy StaticWebEventInit.cs into your project

 
