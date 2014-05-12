#include "StdAfx.h"
#import <Cocoa/Cocoa.h>

OSStatus EvaluateCert (SecCertificateRef cert, CFTypeRef policyRef, SecTrustResultType *result, SecTrustRef *pTrustRef)
{
    OSStatus status1;
    OSStatus status2;
    
    const void* evalCertArray[] = { cert };
    CFArrayRef cfCertRef = CFArrayCreate(kCFAllocatorDefault, evalCertArray, 1, NULL);
    
    if (!cfCertRef)
        return memFullErr;
    
    status1 = SecTrustCreateWithCertificates(cfCertRef, policyRef, pTrustRef);
    if (status1)
        return status1;
    
    status2 = SecTrustEvaluate (*pTrustRef, result);
    
    // Release the objects we allocated
    if (cfCertRef)
        CFRelease(cfCertRef);
    //if (cfDate)
    //    CFRelease(cfDate);
    
    return (status2);
}

extern "C" _AnomalousExport bool CertificateValidator_ValidateSSLCertificate(unsigned char* certBytes, unsigned int certBytesLength, String hostName)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    SecTrustRef trustRef = nil;
    SecTrustResultType result = kSecTrustResultRecoverableTrustFailure;
    bool success = false;
    
    CFStringRef cfHostName = NULL;
    if(hostName != NULL)
    {
        cfHostName = CFStringCreateWithFormat(kCFAllocatorDefault, NULL, CFSTR("%S"), hostName);
        if(cfHostName == NULL)
        {
            return false; //Fail if a host is provided, but cannot be translated. This prevents us trusting something we shouldn't if there is an error here.
        }
    }
    
    NSData *certData = [NSData dataWithBytes:certBytes length:certBytesLength];
    SecCertificateRef cert = SecCertificateCreateWithData(NULL, (CFDataRef)certData);
    if(cert != nil)
    {
        SecPolicyRef policyRef = SecPolicyCreateSSL(false, cfHostName);
        EvaluateCert(cert, policyRef, &result, &trustRef);
        success = result == kSecTrustResultUnspecified;
        
        if(trustRef != nil)
        {
            CFRelease(trustRef);
        }
        
        CFRelease(policyRef);
    }
    
    if(cfHostName != NULL)
    {
        CFRelease(cfHostName);
    }
    
    CFRelease(cert);
    
    [pool drain];
    
    return success;
}

extern "C" _AnomalousExport void MacPlatformConfig_getLocalUserDocumentsFolder(StringRetrieverCallback retrieve)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    NSString* path = [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) lastObject];
    retrieve([path UTF8String]);
    [pool drain];
}

extern "C" _AnomalousExport void MacPlatformConfig_getLocalDataFolder(StringRetrieverCallback retrieve)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    NSString* path = [NSSearchPathForDirectoriesInDomains(NSApplicationSupportDirectory, NSUserDomainMask, YES) lastObject];
    retrieve([path UTF8String]);
    [pool drain];
}

extern "C" _AnomalousExport void MacPlatformConfig_getLocalPrivateDataFolder(StringRetrieverCallback retrieve)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    NSString* path = [NSSearchPathForDirectoriesInDomains(NSApplicationSupportDirectory, NSUserDomainMask, YES) lastObject];
    retrieve([path UTF8String]);
    [pool drain];
}