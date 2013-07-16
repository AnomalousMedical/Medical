#include "StdAfx.h"
#import <Cocoa/Cocoa.h>

extern "C" _AnomalousExport bool ConnectionValidator_ValidateUrl(String url)
{
    NSURL *nsurl = [NSURL URLWithString:[NSString stringWithFormat:@"%S", url]];
    NSURLRequest *request = [[NSURLRequest alloc] initWithURL:nsurl];
    NSError *error = nil;
    NSURLResponse *response = nil;
    
    [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:&error];
    bool success = response != nil;
    [request release];
    return success;
}

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

extern "C" _AnomalousExport bool CertificateValidator_ValidateSSLCertificate(unsigned char* certBytes, unsigned int certBytesLength, const char* hostName)
{
    SecTrustRef trustRef = nil;
    SecTrustResultType result = kSecTrustResultRecoverableTrustFailure;
    bool success = false;
    
    CFStringRef cfHostName = NULL;
    if(hostName != NULL)
    {
        cfHostName = CFStringCreateWithCString(kCFAllocatorDefault, hostName, kCFStringEncodingASCII);
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
    CFRelease(certData);
    
    return success;
}