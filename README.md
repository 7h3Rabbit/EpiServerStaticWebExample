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
  - javascript (no dependencies)
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
- Must allow visits with user-agent `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36 StaticWebPlugin/0.1`


## Installation ##

- Copy StaticWebEventInit.cs into your project
- Change `rootUrl` in StaticWebEventInit.cs to your website url (must allow anonymous access)
- Change `rootPath` in StaticWebEventInit.cs to folder you want to write to (for example a GitHub repository folder)



## How to use ##

- Do changes for a page or block(must be placed on a page) in EpiServer and publish your changes.
- StaticWebPlugin will now try to access your website and write back the result it gets to the folder you have entered.
 
