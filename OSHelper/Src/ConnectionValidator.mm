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