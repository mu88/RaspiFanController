# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="7.0.0"></a>
## [7.0.0](https://www.github.com/mu88/RaspiFanController/releases/tag/7.0.0) (2026-11-27)

### ✨ Features

* migrate code coverage from coverlet to dotnet-coverage ([a859430](https://www.github.com/mu88/RaspiFanController/commit/a85943065023f24f2026cb4a26ed7e472d11a063))
* migrate to .NET 11 ([358f2b9](https://www.github.com/mu88/RaspiFanController/commit/358f2b94a77ab7ac6b089aca1ffa62295ab7f53b))

### 🐛 Bug Fixes

* suppress MA0206 on empty App partial class body ([7ff6938](https://www.github.com/mu88/RaspiFanController/commit/7ff69381e3e86a73615a71fbdb87f24581fef8ed))
* **ci:** update pinned mu88/common SHA to include commit-message quoting fix ([f2bc77b](https://www.github.com/mu88/RaspiFanController/commit/f2bc77b351674e9ca5a4b4873a9d084ad31757cf))
* **container:** bump base image to 10.0.13 to patch runtime CVEs ([b2a4141](https://www.github.com/mu88/RaspiFanController/commit/b2a4141210a8cfae61c2f5ef7b53375c759aeb33))
* **deps:** adopt shared testing publish worker fix ([b39d8f9](https://www.github.com/mu88/RaspiFanController/commit/b39d8f965009dc4ad0ce051aedd77e5a36f65688))
* **security:** patch .NET runtime and test dependency ([3b89a09](https://www.github.com/mu88/RaspiFanController/commit/3b89a0921fb24972ed1628d30a13d8be4ce1ac8f))
* **tests:** use valid SemVer for system test container image tag ([722951b](https://www.github.com/mu88/RaspiFanController/commit/722951bf31993c731a495787068501471f04c9c3))

### ♻️ Refactors

* apply code review findings and add comprehensive tests ([0246dc0](https://www.github.com/mu88/RaspiFanController/commit/0246dc033133da3a3a63196ff39eed120a18f43a))
* replace custom ITaskHelper with built-in TimeProvider ([572f291](https://www.github.com/mu88/RaspiFanController/commit/572f2919aee04d942bbf9e82535b23acd85593ff))
* restructure project to use new component-based architecture ([76720d9](https://www.github.com/mu88/RaspiFanController/commit/76720d96f33fa9063e63f58013cc5e6ea615a8a1))
* update application path base and improve data loading mechanism ([a9b6210](https://www.github.com/mu88/RaspiFanController/commit/a9b6210d51b4c145173e77db407c6e463fe68074))
* **ci:** use centralized pin guard ([7920ad1](https://www.github.com/mu88/RaspiFanController/commit/7920ad1e35d741ea7a2389c1cc4ae2c988915947))
* **tests:** adopt mu88.Shared.Testing for System test infrastructure ([187f566](https://www.github.com/mu88/RaspiFanController/commit/187f5668d2cb6cf82877fd005e2e6117f592e160))

### ✅ Tests

* improve system test ([052627b](https://www.github.com/mu88/RaspiFanController/commit/052627bf06abbeaff03fbd4c2ceafdcacf1e06a9))
* simplify build process by using CliWrap ([b4beec2](https://www.github.com/mu88/RaspiFanController/commit/b4beec2a87ad226fc26f92648b6d15285a7f070a))

### 🔧 Chores

* apply new code style ([0b8544e](https://www.github.com/mu88/RaspiFanController/commit/0b8544e1dad40074a7ed3035fded4e6ff929c112))
* remove IIS Express settings from launchSettings.json ([4fe8371](https://www.github.com/mu88/RaspiFanController/commit/4fe83713b8e6f9885abc7571549da3e39c985eeb))
* resolve Sonar issues ([93126c8](https://www.github.com/mu88/RaspiFanController/commit/93126c8bfeec049f6d2e7dc191286174ce3b6133))
* use new shared GitHub repo mu88/common ([fb92400](https://www.github.com/mu88/RaspiFanController/commit/fb92400b5b4d8291089263e0b42ca4ffbd1aac16))
* **deps:** update actions/checkout action to v7 ([ec914e5](https://www.github.com/mu88/RaspiFanController/commit/ec914e53955392e61a58b31cb76cfad444fea2d2))
* **deps:** update actions/upload-artifact action to v6 ([39b03c2](https://www.github.com/mu88/RaspiFanController/commit/39b03c27a0333b23cd739caef4f71eafa7cef11d))
* **deps:** update all .net ([26c3468](https://www.github.com/mu88/RaspiFanController/commit/26c3468dd6db134ce8c5f28062f1eb7d30b64c44))
* **deps:** update all .net ([48e1470](https://www.github.com/mu88/RaspiFanController/commit/48e14709d90a0df955c6f2cc805da7f39d8e00e9))
* **deps:** update all .net ([7dc9cd1](https://www.github.com/mu88/RaspiFanController/commit/7dc9cd19b001d5c9c1e06420ace5af9165f56569))
* **deps:** update all .net ([2f870ff](https://www.github.com/mu88/RaspiFanController/commit/2f870ff7434554c578ec5866441de5d2d215f7ab))
* **deps:** update all .net ([a187a32](https://www.github.com/mu88/RaspiFanController/commit/a187a3229e1ebb809168e5d06bf3651dcee781f9))
* **deps:** update all .net ([e9b4f1e](https://www.github.com/mu88/RaspiFanController/commit/e9b4f1efc960bc812bd3c26e20607247e81d9316))
* **deps:** update all dependencies ([2138a11](https://www.github.com/mu88/RaspiFanController/commit/2138a1184cd2275499ab4defb6152ba8190f012f))
* **deps:** update all dependencies ([001390c](https://www.github.com/mu88/RaspiFanController/commit/001390cd2331ed988454596564aa1b20204b56b2))
* **deps:** update all dependencies ([2373474](https://www.github.com/mu88/RaspiFanController/commit/237347494b0d8a1934b9d625e592711a7f118368))
* **deps:** update all dependencies ([2f45e19](https://www.github.com/mu88/RaspiFanController/commit/2f45e1969a6c68fc8313357acb2876bf0dfe9fec))
* **deps:** update all dependencies ([e969213](https://www.github.com/mu88/RaspiFanController/commit/e9692131329119a5ad610eb9c89d6ccfb6a6160a))
* **deps:** update all dependencies ([e106c87](https://www.github.com/mu88/RaspiFanController/commit/e106c87a68f9a7a97caee26cceaca5d6745fdb72))
* **deps:** update all dependencies ([185876c](https://www.github.com/mu88/RaspiFanController/commit/185876cb310ea8e34cdcbeeab0c044e185cdaf33))
* **deps:** update all dependencies ([64bc312](https://www.github.com/mu88/RaspiFanController/commit/64bc312c7f2296d03c1fb2f94cbc70fa60202dcd))
* **deps:** update all dependencies ([3e87448](https://www.github.com/mu88/RaspiFanController/commit/3e874485b85b85311b258a68557c34b78f06037b))
* **deps:** update all dependencies ([772a235](https://www.github.com/mu88/RaspiFanController/commit/772a23532691d0c4c76080d48e1ae2685b426303))
* **deps:** update all dependencies ([8f318bc](https://www.github.com/mu88/RaspiFanController/commit/8f318bc645aca5efd94dfe4ce7fda111ee9b1b7c))
* **deps:** update all dependencies ([34fd198](https://www.github.com/mu88/RaspiFanController/commit/34fd19800592ece77f2ec85c3cc97eb05c369a5f))
* **deps:** update all dependencies ([7e97e13](https://www.github.com/mu88/RaspiFanController/commit/7e97e13780d29faf98cfbad1386d1fdfdb6e376f))
* **deps:** update all dependencies ([d4e03e4](https://www.github.com/mu88/RaspiFanController/commit/d4e03e4fb70df841da0563efda05343e0dfc3e05))
* **deps:** update all dependencies ([0a4c263](https://www.github.com/mu88/RaspiFanController/commit/0a4c2633b8ac46cdd8920b22a66cfb103588af44))
* **deps:** update all dependencies ([4daab1d](https://www.github.com/mu88/RaspiFanController/commit/4daab1d2cf858601bf088547ec389aaef254294d))
* **deps:** update all dependencies ([423ee77](https://www.github.com/mu88/RaspiFanController/commit/423ee77f7357fa52f23f0a2cc8ebb135a34f36be))
* **deps:** update all dependencies to 3.10.2 ([16e36de](https://www.github.com/mu88/RaspiFanController/commit/16e36de685a3742b8a2b9cf1a16d28beba2d6761))
* **deps:** update all dependencies to 4.10.0 ([e977ba4](https://www.github.com/mu88/RaspiFanController/commit/e977ba42a0a8c735819c422929bdfc3906537045))
* **deps:** update all dependencies to v18.8.0 ([399ebb0](https://www.github.com/mu88/RaspiFanController/commit/399ebb03b977049a51148f44336817150b9cc2be))
* **deps:** update dependency dotnet-coverage to v18.12.0 ([1aaf369](https://www.github.com/mu88/RaspiFanController/commit/1aaf3698e639d22b7decedd09fb6f0d4aee311cb))
* **deps:** update dependency dotnet-sdk to v10.0.101 ([0b89ac4](https://www.github.com/mu88/RaspiFanController/commit/0b89ac4cf8526a59b9f22561173a0168377e87f6))
* **deps:** update dependency microsoft.net.test.sdk to 18.7.0 ([b720802](https://www.github.com/mu88/RaspiFanController/commit/b720802ef190a79138a18aeaf4dd2cbac57d957b))
* **deps:** update dependency nunit3testadapter to 6.1.0 ([844df09](https://www.github.com/mu88/RaspiFanController/commit/844df0999b827d7e6be1fcf4615bc37c1953f2a0))
* **deps:** update dependency versionize to v2.6.0 ([238f9f9](https://www.github.com/mu88/RaspiFanController/commit/238f9f91a82bb7d13eb604f3cf2249122453a51e))
* **deps:** update mu88.Shared to version 6.0.0 for latest OTEL fixes ([dc7b959](https://www.github.com/mu88/RaspiFanController/commit/dc7b9598c643043e3bdb5fd692d4c16848518dd9))
* **deps:** update mu88.Shared to version 7.0.0 for latest OTEL fixes ([9becdb2](https://www.github.com/mu88/RaspiFanController/commit/9becdb253b00d7435a84092bb378ee3b51e84dfa))
* **deps:** update mu88/common digest to 03ce929 ([5432a42](https://www.github.com/mu88/RaspiFanController/commit/5432a42f9d159e7430edbd23f562edc80ac4cc65))
* **deps:** update mu88/common digest to 0d04e69 ([7a98909](https://www.github.com/mu88/RaspiFanController/commit/7a98909d1c464ff73a02855e3db57fdf3cf6b0ea))
* **deps:** update mu88/common digest to 349db23 ([1a966c6](https://www.github.com/mu88/RaspiFanController/commit/1a966c673194c880888da489dd8ffb1985473ec2))
* **deps:** update mu88/common digest to 5d4855b ([a155e9f](https://www.github.com/mu88/RaspiFanController/commit/a155e9fae6d9759a6adb71bd6319ead4d89f304d))
* **deps:** update mu88/common digest to 88212c6 ([6c444bf](https://www.github.com/mu88/RaspiFanController/commit/6c444bf296b42027a4a6cad792c07adcf50a8195))
* **deps:** update NUnit adapter ([9228cbd](https://www.github.com/mu88/RaspiFanController/commit/9228cbd3eb266f976930a75df6d1ee004bc735ea))
* **deps:** update SDK and package versions to 10.0.102 ([8059146](https://www.github.com/mu88/RaspiFanController/commit/80591461150dfc01f1fc145d09445621a4334b20))
* **deps:** upgrade mu88.Shared for latest OTel / health check updates ([6e9b2a0](https://www.github.com/mu88/RaspiFanController/commit/6e9b2a05bf2952cc31fefad8c20994daed0764f8))
* **deps:** upgrade NUnit to 5.0.0 ([fe53d97](https://www.github.com/mu88/RaspiFanController/commit/fe53d971d7d82d3375933d12b864438a63f2c09c))
* **deps:** use LibMan ([9aec8eb](https://www.github.com/mu88/RaspiFanController/commit/9aec8eb7b0476edfc42b542440b4fa72a30b67ef))
* **dev:** fix broken DotSettings links ([9b8f91b](https://www.github.com/mu88/RaspiFanController/commit/9b8f91bcbcdf34454a7d6ef3b7b13970c8cbd2cd))

### Breaking Changes

* migrate to .NET 11 ([358f2b9](https://www.github.com/mu88/RaspiFanController/commit/358f2b94a77ab7ac6b089aca1ffa62295ab7f53b))
* update application path base and improve data loading mechanism ([a9b6210](https://www.github.com/mu88/RaspiFanController/commit/a9b6210d51b4c145173e77db407c6e463fe68074))

<a name="6.0.1"></a>
## [6.0.1](https://www.github.com/mu88/RaspiFanController/releases/tag/v6.0.1) (2025-12-13)

<a name="6.0.0"></a>
## [6.0.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v6.0.0) (2025-12-12)

### ✨ Features

* upgrade to .NET 10 ([f3cb538](https://www.github.com/mu88/RaspiFanController/commit/f3cb5387810170b4e463cc62d38084d0dc915536))

### ✅ Tests

* harmonize test and build all configured Docker images ([40d280d](https://www.github.com/mu88/RaspiFanController/commit/40d280db2c631cabcec8cd77bdd64787e7f3ddc6))

### 🔧 Chores

* switch to new SLNX format ([958285a](https://www.github.com/mu88/RaspiFanController/commit/958285a3d1ee8f25c21d616cb3a86245b1411df0))
* **deps:** update all dependencies ([e236104](https://www.github.com/mu88/RaspiFanController/commit/e2361045e510b6399ac0f82870cc6eed002c7561))
* **deps:** update all dependencies ([62b5bd5](https://www.github.com/mu88/RaspiFanController/commit/62b5bd50d435b96fddd9c88e4a55048c35b3e8d8))
* **deps:** update all dependencies ([e151ad2](https://www.github.com/mu88/RaspiFanController/commit/e151ad242647d581ee31c4fe4d4202312cc7e7ef))
* **deps:** update all dependencies ([5347fef](https://www.github.com/mu88/RaspiFanController/commit/5347fefa5671e52fd989948a53eca6693a1696f5))
* **deps:** update all dependencies ([#37](https://www.github.com/mu88/RaspiFanController/issues/37)) ([abda30f](https://www.github.com/mu88/RaspiFanController/commit/abda30fd1084fe6947d6b80b8337a12c06151b58))
* **deps:** update dependency nunit3testadapter to 5.2.0 ([70feda6](https://www.github.com/mu88/RaspiFanController/commit/70feda6cf07f9942f0d4273e6fd07408dbb1e2bd))
* **deps:** update mu88/github-actions digest to d09e374 ([e687a52](https://www.github.com/mu88/RaspiFanController/commit/e687a52a66a36525afabeaefc8e5baa288d29509))

### Breaking Changes

* upgrade to .NET 10 ([f3cb538](https://www.github.com/mu88/RaspiFanController/commit/f3cb5387810170b4e463cc62d38084d0dc915536))

<a name="5.0.0"></a>
## [5.0.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v5.0.0) (2025-10-05)

### Breaking Changes

* use GitHub Container Registry instead of Docker Hub ([b2b4cf5](https://www.github.com/mu88/RaspiFanController/commit/b2b4cf512c07575ceea778eb98eb08bb37c7b6ca))

<a name="4.1.0"></a>
## [4.1.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v4.1.0) (2024-12-20)

### Features

* add health check ([d5223bd](https://www.github.com/mu88/RaspiFanController/commit/d5223bdbf7dad7ab370af56419928593cbe514fd))
* embed health check tool ([642e78a](https://www.github.com/mu88/RaspiFanController/commit/642e78a3b76186074beb8d45a08f0883aa5e9c6e))

<a name="4.0.0"></a>
## [4.0.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v4.0.0) (2024-12-06)

### Features

* **deps:** upgrade to .NET 9 ([3cce48d](https://www.github.com/mu88/RaspiFanController/commit/3cce48dcc7220166705e0bc37910f5035fb7c45b))

### Breaking Changes

* **deps:** upgrade to .NET 9 ([3cce48d](https://www.github.com/mu88/RaspiFanController/commit/3cce48dcc7220166705e0bc37910f5035fb7c45b))

<a name="3.3.0"></a>
## [3.3.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.3.0) (2024-08-03)

### Features

* replace OpenTelemetry and multi-manifest Docker image logic with NuGet package mu88.Shared ([283aac9](https://www.github.com/mu88/RaspiFanController/commit/283aac9c434a196fa6c795d2242aedcfe26943db))

<a name="3.2.1"></a>
## [3.2.1](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.2.1) (2024-06-29)

<a name="3.2.0"></a>
## [3.2.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.2.0) (2024-06-16)

### Features

* add OpenTelemetry ([71bda31](https://www.github.com/mu88/RaspiFanController/commit/71bda312c36bcec92abcbd2da56a18739e2f6bab))

<a name="3.1.0"></a>
## [3.1.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.1.0) (2024-01-21)

### Features

* use built-in temperature provider ([d1babb8](https://www.github.com/mu88/RaspiFanController/commit/d1babb836cc8dc88d16e598c863b63b1eb54bbf0))

<a name="3.0.3"></a>
## [3.0.3](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.0.3) (2024-01-06)

<a name="3.0.2"></a>
## [3.0.2](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.0.2) (2023-12-01)

<a name="3.0.1"></a>
## [3.0.1](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.0.1) (2023-11-17)

### Bug Fixes

* run as root user ([a262005](https://www.github.com/mu88/RaspiFanController/commit/a2620053acfe99f2cf8de1c72e0ed51965f1da1a))

<a name="3.0.0"></a>
## [3.0.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v3.0.0) (2023-11-17)

### Features

* use .NET 8 ([a85230e](https://www.github.com/mu88/RaspiFanController/commit/a85230e5d5652801569659b75adf144837eed393))

### Breaking Changes

* use .NET 8 ([a85230e](https://www.github.com/mu88/RaspiFanController/commit/a85230e5d5652801569659b75adf144837eed393))

<a name="2.3.0"></a>
## [2.3.0](https://www.github.com/mu88/RaspiFanController/releases/tag/v2.3.0) (2023-11-17)

### Features

* upgrade to .NET 7 ([bb1ac79](https://www.github.com/mu88/RaspiFanController/commit/bb1ac7984d1edf65b2ed988fa92bfdcab38d20e7))

