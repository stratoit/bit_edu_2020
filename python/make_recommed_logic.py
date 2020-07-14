# Lab 6 Softmax Classifier
import pymssql
import tensorflow as tf
import numpy as np
tf.set_random_seed(777)  # for reproducibility

############데이터 가져오기
conn = pymssql.connect(server='20.41.81.89', user='bit', password='protec21', database='mb_store')

cursor = conn.cursor()

cursor.execute("SELECT sp.quantity, YEAR(GETDATE()) - YEAR(c.birth) AS 나이, c.gender, DATEDIFF(month, sh.sales_date, GETDATE()) AS 개월전, sp.product_id FROM sales_product sp "
               "JOIN sales_history sh ON sp.sales_history_id = sh.sales_history_id "
               "JOIN customer c ON sh.customer_id = c.customer_id "
               "WHERE sh.refunded = 0")

row = cursor.fetchone()
dbdata =[]
dbSet = set()
i=0
while row:
    gender=1
    if row[2] == "여성":
        gender = 0
    data = [ row[0], row[1], gender, row[3], row[4] ]
    dbdata.append(data)

    dbSet.add(row[-1])
    row = cursor.fetchone()

conn.close()

#########x데이터만들기
datas = []
for data in dbdata:
    datas.append([data[1],data[2],data[3],data[4]])
    for i in range(1,data[0]):
        datas.append([data[1],data[2],data[3],data[4]])

#########y데이터만들기
dbSetList = list(dbSet)
dbDict = dict()
for i in range(len(dbSetList)):
    dbDict[dbSetList[i]] = i

y_data =[]
for data in datas:
    y_data.append([dbDict[data[-1]]])

###########담기
xy = np.array(datas)
x_data = xy[:, 0:-1]
y_data = np.array(y_data)

print(x_data.shape, y_data.shape)


##############초기설정
nb_classes = len(dbSet)  # 결과갯수

X = tf.placeholder(tf.float32, [None, 3])
Y = tf.placeholder(tf.int32, [None, 1])

Y_one_hot = tf.one_hot(Y, nb_classes)  # one hot
print("one_hot", Y_one_hot)
Y_one_hot = tf.reshape(Y_one_hot, [-1, nb_classes])
print("reshape", Y_one_hot)


W = tf.Variable(tf.random_normal([3, nb_classes]), name='weight')
b = tf.Variable(tf.random_normal([nb_classes]), name='bias')

logits = tf.matmul(X, W) + b
hypothesis = tf.nn.softmax(logits)

# Cross entropy cost/loss
cost_i = tf.nn.softmax_cross_entropy_with_logits_v2(logits=logits,
                                                 labels=tf.stop_gradient([Y_one_hot]))
cost = tf.reduce_mean(cost_i)
optimizer = tf.train.GradientDescentOptimizer(learning_rate=0.1).minimize(cost)

prediction = tf.argmax(hypothesis, 1)
correct_prediction = tf.equal(prediction, tf.argmax(Y_one_hot, 1))
accuracy = tf.reduce_mean(tf.cast(correct_prediction, tf.float32))
# Launch graph
with tf.Session() as sess:
    sess.run(tf.global_variables_initializer())

    for step in range(2000):
        sess.run(optimizer, feed_dict={X: x_data, Y: y_data})
        if step % 100 == 0:
            loss, acc = sess.run([cost, accuracy], feed_dict={
                                 X: x_data, Y: y_data})
            print("Step: {:5}\tLoss: {:.3f}\tAcc: {:.2%}".format(
                step, loss, acc))

    # 결과확인
    # Let's see if we can predict
    pred = sess.run(prediction, feed_dict={X: x_data})
    # y_data: (N,1) = flatten => (N, ) matches pred.shape
    for p, y in zip(pred, y_data.flatten()):
        print("[{}] Prediction: {} True Y: {}".format(p == int(y), p, int(y)))
    saver = tf.train.Saver()
    saver.save(sess, "./recom.cpkt")

    print("학습된 모델을 저장했습니다.")