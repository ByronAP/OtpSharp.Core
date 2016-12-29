# OtpSharp.Core
OtpSharp for .NET Core/NetStandard original code at https://bitbucket.org/devinmartin/otp-sharp/
 
Since protected memory is not available cross platorm it has been removed.
This library is completely untested so please report any issues.
 
_
An implementation of HOTP (RFC 4226) and TOTP (RFC 6238) in C#.
http://tools.ietf.org/html/rfc6238
http://www.ietf.org/rfc/rfc4226.txt
This library is released under an MIT license. No warranty is provided as to the correctness of the library and the consumer of the library assumes all risk for the use thereof, as per the MIT license.
Every effort has gone into implementing this library in accordance with the RFCs mentioned above. However it is up to the users of this library to read through the RFCs and ensure that this implementation is in accordance with the security procedures outlined therein.
The implementation includes the code calculation as well as simple verification. Persistence of the keys, secure key storage, ensuring that a single code can't be validated multiple times and other things are not a part of this library. Thus far those functions (and any others mentioned in the RFCs) are up to the consumer of this library.
Warning this library is implemented in a managed language. Reasonable efforts have been taken to limit the exposure of the key to memory dumps however it is impossible to fully protect against this sort of thing in purely managed code. The best way to ensure that the key isn't leaked is to use an HSM or similar hardened cryptographic provider Key Providers. If the key is brought into managed code (which is the default) then the garbage collector can move objects (including the key while temporarily in plain text form) around without warning preventing complete cleanup of the plain text key that is attempted after the key is used.
The RFC recommends using a hardware security module for maximum protection against key leakage. This library supports using an HSM or other key provider to do the HMAC computation and doing only the truncation/formatting in managed code though it requires custom implementation, see Key Providers. The consumer of this library assumes all risk and responsibility for all security flaws, both in the implementation of the library (if any), as well as those cases where the platform prevents full compliance with the RFC.
