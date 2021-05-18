<p align="center">
  <a href="https://Vauth.org/">
      <img
      src="https://drive.google.com/file/d/1MZVO6MKGjda8WnALaD0UxeUOy5Cw4S34/view?usp=sharing"
      width="250px" alt="Vauth-logo">
  </a>
</p>

<h3 align="center">Vauth Blockchain</h3>

<p align="center">
   A modern distributed network for the Smart Economy.
  <br>
  <a href="https://docs.Vauth.org/docs/en-us/index.html"><strong>Documentation »</strong></a>
  <br>
  <br>
  <a href="https://github.com/Vauth-project/Vauth"><strong>Vauth</strong></a>
  ·
  <a href="https://github.com/Vauth-project/Vauth-vm">Vauth VM</a>
  ·
  <a href="https://github.com/Vauth-project/Vauth-modules">Vauth Modules</a>
  ·
  <a href="https://github.com/Vauth-project/Vauth-devpack-dotnet">Vauth DevPack</a>
  ·
  <a href="https://github.com/Vauth-project/Vauth-node">Vauth Node</a>
</p>
<p align="center">
  <a href="https://twitter.com/Vauth_blockchain">
      <img
      src=".github/images/twitter-logo.png"
      width="25px">
  </a>
  &nbsp;
  <a href="https://medium.com/Vauth-smart-economy">
      <img
      src=".github/images/medium-logo.png"
      width="23px">
  </a>
  &nbsp;
  <a href="https://Vauthnewstoday.com">
      <img
      src=".github/images/nnt-logo.jpg"
      width="23px">
  </a>
  &nbsp;  
  <a href="https://t.me/Vauth_EN">
      <img
      src=".github/images/telegram-logo.png"
      width="24px" >
  </a>
  &nbsp;
  <a href="https://www.reddit.com/r/Vauth/">
      <img
      src=".github/images/reddit-logo.png"
      width="24px">
  </a>
  &nbsp;
  <a href="https://discord.io/Vauth">
      <img
      src=".github/images/discord-logo.png"
      width="25px">
  </a>
  &nbsp;
  <a href="https://www.youtube.com/Vauthsmarteconomy">
      <img
      src=".github/images/youtube-logo.png"
      width="32px">
  </a>
  &nbsp;
  <!--How to get a link? -->
  <a href="https://Vauth.org/">
      <img
      src=".github/images/we-chat-logo.png"
      width="25px">
  </a>
  &nbsp;
  <a href="https://weibo.com/Vauthsmarteconomy">
      <img
      src=".github/images/weibo-logo.png"
      width="28px">
  </a>
</p>
<p align="center">
  <a href="https://travis-ci.org/Vauth-project/Vauth">
    <img src="https://travis-ci.org/Vauth-project/Vauth.svg?branch=master" alt="Current TravisCI build status.">
  </a>
  <a href="https://github.com/Vauth-project/Vauth/releases">
    <img src="https://badge.fury.io/gh/Vauth-project%2FVauth.svg" alt="Current Vauth version.">
  </a>
  <a href='https://coveralls.io/github/Vauth-project/Vauth'>
    <img src='https://coveralls.io/repos/github/Vauth-project/Vauth/badge.svg' alt='Coverage Status' />
  </a>
  <a href="https://github.com/Vauth-project/Vauth/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License.">
  </a>
</p>




## Table of Contents
1. [Overview](#overview)
2. [Project structure](#project-structure)
3. [Related projects](#related-projects)
4. [Opening a new issue](#opening-a-new-issue)  
5. [Bounty program](#bounty-program)
6. [License](#license)

## Overview
This repository contain main classes of the 
[Vauth](https://www.Vauth.org) blockchain.   
Visit the [documentation](https://docs.Vauth.org/docs/en-us/index.html) to get started.


*Note: This is Vauth 3 branch, currently under development. For the current stable version, please [click here.](https://github.com/Vauth-project/Vauth/tree/master-2.x)*



## Project structure
An overview of the project folders can be seen below.

|Folder|Content|
|---|---|
|Consensus| Classes used in the dBFT consensus algorithm, including the `ConsensusService` actor.|
|Cryptography|General cryptography classes including ECC implementation.|
|IO|Data structures used for caching and collection interaction.|
|Ledger|Classes responsible for the state control, including the `MemoryPool` and `Blockchain` classes.|
|Network|Peer-to-peer protocol implementation classes.|
|Persistence|Classes used to allow other classes to access application state.|
|Plugins|Interfaces used to extend Vauth, including the storage interface.|
|SmartContract|Native contracts, `ApplicationEngine`, `InteropService` and other smart-contract related classes.|
|VM|Helper methods used to interact with the VM.|
|Wallet|Wallet and account implementation. |


## Related projects
Code references are provided for all platform building blocks. That includes the base library, the VM, a command line application and the compiler. 

* [Vauth:](https://github.com/Vauth-project/Vauth/) Vauth core library, contains base classes, including ledger, p2p and IO modules.
* [Vauth-vm:](https://github.com/Vauth-project/Vauth-vm/) Vauth Virtual Machine is a decoupled VM that Vauth uses to execute its scripts. It also uses the `InteropService` layer to extend its functionalities.
* [Vauth-node:](https://github.com/Vauth-project/Vauth-node/) Executable version of the Vauth library, exposing features using a command line application or GUI.
* [Vauth-modules:](https://github.com/Vauth-project/Vauth-modules/) Vauth modules include additional tools and plugins to be used with Vauth.
* [Vauth-devpack-dotnet:](https://github.com/Vauth-project/Vauth-devpack-dotnet/) These are the official tools used to convert a C# smart-contract into a *Vauth executable file*.

## Opening a new issue
Please feel free to create new issues to suggest features or ask questions.

- [Feature request](https://github.com/Vauth-project/Vauth/issues/new?assignees=&labels=discussion&template=feature-or-enhancement-request.md&title=)
- [Bug report](https://github.com/Vauth-project/Vauth/issues/new?assignees=&labels=&template=bug_report.md&title=)
- [Questions](https://github.com/Vauth-project/Vauth/issues/new?assignees=&labels=question&template=questions.md&title=)

If you found a security issue, please refer to our [security policy](https://github.com/Vauth-project/Vauth/security/policy).

## Bounty program
You can be rewarded by finding security issues. Please refer to our [bounty program page](https://Vauth.org/bounty) for more information.

## License
The Vauth project is licensed under the [MIT license](LICENSE).
