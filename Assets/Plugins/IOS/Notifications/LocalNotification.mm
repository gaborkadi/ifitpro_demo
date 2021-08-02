//
//  LocalNotification.m
//  LocalNotificationObj
//
//  Created by Yaroslav on 3/7/16.
//  Copyright © 2016 Area730. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "LocalNotificationObj.h"
#import "AppDelegateListener.h"


@implementation LocalNotificationObj

////////////    CONSTANS    /////////////

float vertion = 1.2f;

static NSString * const key_id = @"key_id";

///////////     END OF CONSTANS     //////////////

//subscribe for unity callback
- (id)init {
    if ((self = [super init])) {
        NSLog(@"Subscibing for AppDelegateListener: kUnityDidReceiveLocalNotification");
        NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
        [notificationCenter addObserver: self
                               selector: @selector (handleLocalNotificationEvent:)
                                   name: kUnityDidReceiveLocalNotification
                                 object: nil];
        
    }
    
    return self;
}


//Called when notification is opened by ckicking on it when it appears to user
- (void) handleLocalNotificationEvent: (NSNotification *) receivedNotification
{
    NSLog(@"handleLocalNotificationEvent My");
    [self applicationIconBadgeNumber:0];
}



////////////////////////////////        ///////////////////////////////////////////////

-(void) setUpLocalNotification  :(NSString*) notification_id
                                :(NSString*) title
                                :(NSString*)body
                                :(NSString*)badge_number
                                :(NSString*)custome_sound
                                :(bool) is_repeating
                                :(int)repeat_unit
                                :(long)delay


{
    
    NSLog(@"Ask for permition");
    if ([UIApplication instancesRespondToSelector:@selector(registerUserNotificationSettings:)]) {
        [[UIApplication sharedApplication] registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:UIUserNotificationTypeAlert|UIUserNotificationTypeSound categories:nil]];
    }
    
    NSLog(@"Start setUpLocalNotification");
    
    
    
    UILocalNotification* n1 = [[UILocalNotification alloc] init];
    
    n1.timeZone             = [NSTimeZone defaultTimeZone];
    n1.alertTitle           = title;
    n1.alertBody            = body;
    n1.fireDate             = [NSDate dateWithTimeIntervalSinceNow: delay];
    
    
    if(is_repeating)
    {
        NSLog(@"Repeating unit %d", repeat_unit);
        switch(repeat_unit)
        {
            default:n1.repeatInterval = NSDayCalendarUnit;break;
                
            case 0: n1.repeatInterval = NSMinuteCalendarUnit;break;
            case 1: n1.repeatInterval = NSHourCalendarUnit;break;
            case 2: n1.repeatInterval = NSDayCalendarUnit;break;
            case 3: n1.repeatInterval = NSMonthCalendarUnit;break;
            case 4: n1.repeatInterval = NSYearCalendarUnit;break;
                
        }
        
    }
    
    //custome sound
    
    NSLog(@"Sound name: %@", custome_sound);
    
    if([custome_sound isEqual:@""])
    {
        n1.soundName = UILocalNotificationDefaultSoundName;
    }
    else
    {
        NSMutableString *test = [custome_sound stringByAppendingString:@".wav"];
        NSMutableString *f = [@"Data/Raw/" stringByAppendingString: test];
        NSLog(@"Sound name: %@", f);
        n1.soundName =  f;
    }
    
    NSLog(@"Badge number string: %@", badge_number);
    
    //
    if([badge_number integerValue] > 0)
    {
        NSLog(@"Set badge number: %@",badge_number);
        n1.applicationIconBadgeNumber = badge_number.intValue;
    }
    
    //set up notification id
    NSMutableDictionary *userInfo = [NSMutableDictionary dictionary];
    [userInfo setObject:notification_id forKey:key_id];
    n1.userInfo = userInfo;
    
    [[UIApplication sharedApplication] scheduleLocalNotification: n1];
    NSLog(@"Finish setUpLocalNotification");
}

-(void)cancelNotificationById:(NSString*) notificationId
{
    NSLog(@"Cancel notification by id: ");
    NSLog(@"%@",notificationId);
    
    UILocalNotification* notification = [self getNotificationById: notificationId];
    
    if (notification) {
        [self minusApplicationIconBadgeNumber:1];
        [[UIApplication sharedApplication]cancelLocalNotification:notification];
    }
    
}
-(void)cancelAllNotification
{
    NSLog(@"cancelAllNotification");
    [self applicationIconBadgeNumber:0];
    [[UIApplication sharedApplication] cancelAllLocalNotifications];
    
}



-(void)clearAllNotification
{
    NSLog(@"clearAllNotification");
    [self applicationIconBadgeNumber:0];
    [UIApplication sharedApplication].scheduledLocalNotifications = [UIApplication sharedApplication].scheduledLocalNotifications;}

/////////////////////////   UTILITY     ///////////////////////////

-(float)getVersion
{
    return vertion;
}

//made for debug purpose
-(void) showToast:(NSString*)message
{
    UIAlertView *toast = [[UIAlertView alloc] initWithTitle:nil
                                                    message:message
                                                   delegate:nil
                                          cancelButtonTitle:nil
                                          otherButtonTitles:nil, nil];
    
    toast.backgroundColor=[UIColor redColor];
    [toast show];
    int duration = 2; // duration in seconds
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, duration * NSEC_PER_SEC), dispatch_get_main_queue(), ^{[toast dismissWithClickedButtonIndex:0 animated:YES];});
}

- (void) applicationIconBadgeNumber:(int) badges
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = badges;
}

- (void) minusApplicationIconBadgeNumber:(int) badges
{
    int current  = [UIApplication sharedApplication].applicationIconBadgeNumber;
    
    if((current - badges) < 0)
    {
        [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
        
    }
    else
    {
        
        [UIApplication sharedApplication].applicationIconBadgeNumber -= badges;
    }
}

-(UILocalNotification *)getNotificationById:(NSString *)notificationId {
    for (UILocalNotification *notification in [[UIApplication sharedApplication] scheduledLocalNotifications]) {
        if ([[notification.userInfo objectForKey:key_id] isEqualToString:notificationId]) {
            return notification;
        }
    }
    
    return nil;
}


@end


static LocalNotificationObj *localNotificationObj = nil;

LocalNotificationObj* getLocaNotification()
{
    if(localNotificationObj == nil)
    {
        localNotificationObj = [[LocalNotificationObj alloc]init];
    }
    
    return localNotificationObj;
    
}


// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}




// Exported interface to Unity


extern "C"
{
    
    int _setUpLocalNotification(
                                const char* notification_id,
                                const char * title,
                                const char * body,
                                const char* badge_number,
                                const char* custome_sound,
                                bool is_repeating,
                                int repeat_unit,
                                int delay
                                )
    {
        getLocaNotification();
        
        [localNotificationObj setUpLocalNotification
         :CreateNSString(notification_id)
         :CreateNSString(title)
         :CreateNSString(body)
         :CreateNSString(badge_number)
         :CreateNSString(custome_sound)
         :is_repeating
         :repeat_unit
         :delay
         
         
         ];
        
        NSLog(@"Badge in C: %@", CreateNSString(badge_number));
        
        return 0;
    }
    
    void _cancelNotificationById(const char* notification_id)
    {
        getLocaNotification();
        [localNotificationObj cancelNotificationById:CreateNSString(notification_id)];
    }
    
    void _cancelAllNotification()
    {
        getLocaNotification();
        [localNotificationObj cancelAllNotification];
    }
    
    
    void _clearAllNotification()
    {
        getLocaNotification();
        [localNotificationObj clearAllNotification];
        
    }
    
    float _getVersion()
    {
        getLocaNotification();
        return [localNotificationObj getVersion];
    }
    
    void _showToast(const char *text)
    {
        getLocaNotification();
        [localNotificationObj showToast:CreateNSString(text)];
        
    }
    
    void _applicationIconBadgeNumber(const int badges)
    {
        getLocaNotification();
        [localNotificationObj applicationIconBadgeNumber:badges];
    }
    
    
}









